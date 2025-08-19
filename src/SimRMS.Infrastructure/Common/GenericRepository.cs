using LB.DAL.Core.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Shared.Models;
using System.Data.Common;
using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       Generic Repository
/// Author:      Md. Raihan Sharif
/// Purpose:     Facilitate Common Database Operations with Enhanced Features for SPs and Bulk Operations
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// Raihan Sharif     11/Aug/2025   Table Value Parameter support added & merged parameters updated
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 


namespace SimRMS.Infrastructure.Common;
/// <summary>
/// Enhanced Generic Repository with SP support and bulk operations
/// </summary>
public class GenericRepository : IGenericRepository
{
    private readonly ILB_DAL _dal;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenericRepository> _logger;
    private readonly string _connectionString;

    public GenericRepository([FromKeyedServices("DBApplication")] ILB_DAL dal, IUnitOfWork unitOfWork, ILogger<GenericRepository> logger, IConfiguration configuration)
    {
        _dal = dal;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection string is required");
    }

    public async Task<T?> QuerySingleAsync<T>(string sqlOrSp, object? parameters = null, bool isStoredProcedure = false, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogDebug("QuerySingle: {SqlOrSp}, SP: {IsStoredProcedure}", sqlOrSp, isStoredProcedure);

        await _unitOfWork.EnsureConnectionAsync(cancellationToken);

        var dalParams = ConvertToDalParams(parameters);
        var commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

        using var reader = await _dal.LB_GetDbDataReaderAsync(sqlOrSp, dalParams, commandType);

        if (await reader.ReadAsync())
        {
            return MapFromReader<T>(reader);
        }

        return null;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sqlOrSp, object? parameters = null, bool isStoredProcedure = false, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogDebug("Query: {SqlOrSp}, SP: {IsStoredProcedure}", sqlOrSp, isStoredProcedure);

        await _unitOfWork.EnsureConnectionAsync(cancellationToken);

        var dalParams = ConvertToDalParams(parameters);
        var commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

        var results = new List<T>();
        using var reader = await _dal.LB_GetDbDataReaderAsync(sqlOrSp, dalParams, commandType);

        while (await reader.ReadAsync())
        {
            results.Add(MapFromReader<T>(reader));
        }

        return results;
    }

    /// <summary>
    /// Enhanced pagination supporting both inline SQL and Stored Procedures
    /// </summary>
    public async Task<PagedResult<T>> QueryPagedAsync<T>(string sqlOrSp, int pageNumber, int pageSize, object? parameters = null, string? orderBy = null, bool isStoredProcedure = false, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogDebug("QueryPaged: {SqlOrSp}, SP: {IsStoredProcedure}, Page: {PageNumber}, Size: {PageSize}", sqlOrSp, isStoredProcedure, pageNumber, pageSize);

        // Validate pagination parameters
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 1000) pageSize = 10;

        await _unitOfWork.EnsureConnectionAsync(cancellationToken);

        if (isStoredProcedure)
        {
            return await QueryPagedFromStoredProcedureAsync<T>(sqlOrSp, pageNumber, pageSize, parameters, cancellationToken);
        }
        else
        {
            return await QueryPagedFromInlineSqlAsync<T>(sqlOrSp, pageNumber, pageSize, parameters, orderBy, cancellationToken);
        }
    }

    public async Task<int> ExecuteAsync(string sqlOrSp, object? parameters = null, bool isStoredProcedure = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Execute: {SqlOrSp}, SP: {IsStoredProcedure}", sqlOrSp, isStoredProcedure);

        await _unitOfWork.EnsureConnectionAsync(cancellationToken);

        var dalParams = ConvertToDalParams(parameters);
        var commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

        // Check if we have any OUTPUT parameters (for backward compatibility, return first OUTPUT param value if exists)
        if (isStoredProcedure && dalParams != null && HasOutputParameters(parameters))
        {
            var result = await ExecuteWithOutputAsync(sqlOrSp, parameters, cancellationToken);
            return result.OutputValues.FirstOrDefault()?.Value is int intValue ? intValue : result.RowsAffected;
        }

        return await _dal.LB_ExecuteNonQueryAsync(sqlOrSp, dalParams, commandType);
    }

    public async Task<T> ExecuteScalarAsync<T>(string sqlOrSp, object? parameters = null, bool isStoredProcedure = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("ExecuteScalar: {SqlOrSp}, SP: {IsStoredProcedure}", sqlOrSp, isStoredProcedure);

        await _unitOfWork.EnsureConnectionAsync(cancellationToken);

        var dalParams = ConvertToDalParams(parameters);
        var commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

        var result = await _dal.LB_ExecuteScalarAsync(sqlOrSp, dalParams, commandType);
        return (T)Convert.ChangeType(result, typeof(T));
    }

    public async Task<int> ExecuteBatchAsync(IEnumerable<(string sql, object? parameters)> commands, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("ExecuteBatch: {CommandCount} commands", commands.Count());

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            int totalAffected = 0;
            foreach (var (sql, parameters) in commands)
            {
                totalAffected += await ExecuteAsync(sql, parameters, false, cancellationToken);
            }
            return totalAffected;
        }, cancellationToken);
    }

    /// <summary>
    /// Bulk insert from entities using SqlBulkCopy
    /// </summary>
    public async Task<int> BulkInsertAsync<T>(IEnumerable<T> entities, string tableName, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogDebug("BulkInsert: {TableName}, Count: {Count}", tableName, entities.Count());

        var dataTable = ConvertToDataTable(entities);
        return await BulkInsertDataTableAsync(dataTable, tableName, cancellationToken);
    }

    /// <summary>
    /// Bulk insert from DataTable using SqlBulkCopy
    /// </summary>
    public async Task<int> BulkInsertDataTableAsync(DataTable dataTable, string tableName, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("BulkInsertDataTable: {TableName}, Rows: {RowCount}", tableName, dataTable.Rows.Count);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            using var bulkCopy = new SqlBulkCopy(_connectionString, SqlBulkCopyOptions.Default);
            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BatchSize = 1000;
            bulkCopy.BulkCopyTimeout = 300; // 5 minutes

            // Auto-map columns based on DataTable column names
            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
            return dataTable.Rows.Count;
        }, cancellationToken);
    }

    /// <summary>
    /// Bulk insert from file (CSV, etc.) with custom line mapper
    /// </summary>
    public async Task<int> BulkInsertFromFileAsync<T>(string filePath, string tableName, Func<string[], T> lineMapper, bool hasHeader = true, string delimiter = ",", CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogDebug("BulkInsertFromFile: {FilePath} -> {TableName}", filePath, tableName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var entities = new List<T>();
        var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);

        var startIndex = hasHeader ? 1 : 0;
        for (int i = startIndex; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var fields = lines[i].Split(delimiter);
            try
            {
                var entity = lineMapper(fields);
                entities.Add(entity);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse line {LineNumber}: {Line}", i + 1, lines[i]);
                // Continue processing other lines
            }
        }

        return await BulkInsertAsync(entities, tableName, cancellationToken);
    }

    #region Private Helper Methods

    /// <summary>
    /// Handle pagination for inline SQL queries
    /// </summary>
    private async Task<PagedResult<T>> QueryPagedFromInlineSqlAsync<T>(string baseQuery, int pageNumber, int pageSize, object? parameters, string? orderBy, CancellationToken cancellationToken) where T : class
    {
        // Default ordering if not specified
        if (string.IsNullOrEmpty(orderBy))
        {
            orderBy = "ORDER BY (SELECT NULL)"; // SQL Server requires ORDER BY for OFFSET
        }
        else if (!orderBy.ToUpper().StartsWith("ORDER BY"))
        {
            orderBy = $"ORDER BY {orderBy}";
        }

        var offset = (pageNumber - 1) * pageSize;

        // Build single query with window function for total count
        var pagedQuery = $@"
                WITH PagedData AS (
                    SELECT *, COUNT(*) OVER() AS TotalCount
                    FROM ({baseQuery}) AS BaseQuery
                    {orderBy}
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                )
                SELECT * FROM PagedData";

        // Add pagination parameters
        var allParameters = MergeParameters(parameters, new { Offset = offset, PageSize = pageSize });
        var dalParams = ConvertToDalParams(allParameters);

        var results = new List<T>();
        int totalCount = 0;

        using var reader = await _dal.LB_GetDbDataReaderAsync(pagedQuery, dalParams, CommandType.Text);

        while (await reader.ReadAsync())
        {
            // Get total count from first row (window function result)
            if (totalCount == 0 && HasColumn(reader, "TotalCount"))
            {
                totalCount = reader.GetInt32("TotalCount");
            }

            // Map the entity (excluding TotalCount column)
            results.Add(MapFromReader<T>(reader));
        }

        return new PagedResult<T>
        {
            Data = results,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }


    /// <summary>
    /// Handle pagination for Stored Procedures with proper OUTPUT parameter support
    /// SP should return data with TotalCount column OR use output parameter
    /// </summary>
    private async Task<PagedResult<T>> QueryPagedFromStoredProcedureAsync<T>(string storedProcedureName, int pageNumber, int pageSize, object? parameters, CancellationToken cancellationToken) where T : class
    {
        // Add pagination parameters to SP parameters
        var spParameters = MergeParameters(parameters, new
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = 0 // Will be set as output parameter
        });

        var results = new List<T>();
        int totalCount = 0;

        // Convert parameters with proper direction settings for OUTPUT parameters
        var dalParams = ConvertToDalParamsWithDirection(spParameters);

        _logger.LogDebug("Executing SP {StoredProcedure} with {ParamCount} parameters",
            storedProcedureName, dalParams?.Count ?? 0);

        // FIXED: Single execution that handles both data AND output parameters
        using var reader = await _dal.LB_GetDbDataReaderAsync(storedProcedureName, dalParams, CommandType.StoredProcedure);

        // Read data from result set
        while (await reader.ReadAsync())
        {
            // Try to get total count from column (if SP includes it in result set)
            if (totalCount == 0 && HasColumn(reader, "TotalCount"))
            {
                totalCount = reader.GetInt32("TotalCount");
            }

            results.Add(MapFromReader<T>(reader));
        }

        // FIXED: After DataReader is consumed, OUTPUT parameters are now available
        // Extract OUTPUT parameter values from dalParams (they are populated by LB_DAL after execution)
        if (totalCount == 0 && dalParams != null)
        {
            var totalCountParam = dalParams.FirstOrDefault(p =>
                p.ParameterName.Equals("TotalCount", StringComparison.OrdinalIgnoreCase) &&
                (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput));

            if (totalCountParam?.Value != null && totalCountParam.Value != DBNull.Value)
            {
                totalCount = Convert.ToInt32(totalCountParam.Value);
                _logger.LogDebug("Retrieved TotalCount from OUTPUT parameter: {TotalCount}", totalCount);
            }
        }

        // Fallback: If SP has second result set with just total count 
        // Important: now we returnt 2 restults set for show the total count (cause lb db reader not handle output param). so total count now get here.
        if (totalCount == 0 && await reader.NextResultAsync())
        {
            if (await reader.ReadAsync())
            {
                // Try to get total count from column ,SP includes it in 2nd result set
                if (totalCount == 0 && HasColumn(reader, "TotalCount"))
                {
                    totalCount = reader.GetInt32("TotalCount");
                }
                else
                    totalCount = reader.GetInt32(0); // First column should be count

                _logger.LogDebug("Retrieved TotalCount from second result set: {TotalCount}", totalCount);
            }
        }

        _logger.LogDebug("SP {StoredProcedure} returned {DataCount} records, TotalCount: {TotalCount}",
            storedProcedureName, results.Count, totalCount);

        return new PagedResult<T>
        {
            Data = results,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// FIXED: Improved OUTPUT parameter detection
    /// </summary>
    private bool HasOutputParameters(object? parameters)
    {
        if (parameters == null) return false;

        var properties = parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        return properties.Any(p => IsOutputParameter(p.Name));
    }

    /// <summary>
    /// FIXED: Better OUTPUT parameter naming pattern detection
    /// </summary>
    private bool IsOutputParameter(string parameterName)
    {
        // Common OUTPUT parameter naming patterns
        var outputPatterns = new[] {
        "RowsAffected", "StatusCode", "StatusMsg", "TotalCount",
        "Result", "ReturnValue", "Count", "Total", "RecordCount"
    };

        return outputPatterns.Any(pattern =>
            parameterName.Equals(pattern, StringComparison.OrdinalIgnoreCase) ||
            parameterName.EndsWith(pattern, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// FIXED: Enhanced parameter conversion with proper direction setting
    /// </summary>
    private List<LB_DALParam>? ConvertToDalParamsWithDirection(object? parameters)
    {
        if (parameters == null) return null;

        var dalParams = new List<LB_DALParam>();

        // Handle Dictionary<string, object>
        if (parameters is Dictionary<string, object> dict)
        {
            foreach (var kvp in dict)
            {
                var value = kvp.Value ?? DBNull.Value;
                var direction = IsOutputParameter(kvp.Key) ? ParameterDirection.Output : ParameterDirection.Input;

                // For OUTPUT parameters, ensure we have a proper initial value for size allocation
                if (direction == ParameterDirection.Output && kvp.Key.Contains("Count", StringComparison.OrdinalIgnoreCase))
                {
                    value = 0; // For integer OUTPUT parameters
                }

                dalParams.Add(new LB_DALParam(kvp.Key, value, direction));
            }
            return dalParams;
        }

        // Handle anonymous objects and POCOs
        var properties = parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            var value = prop.GetValue(parameters) ?? DBNull.Value;
            var direction = IsOutputParameter(prop.Name) ? ParameterDirection.Output : ParameterDirection.Input;

            // For OUTPUT parameters, ensure we have a proper initial value for size allocation
            if (direction == ParameterDirection.Output)
            {
                if (prop.Name.Contains("Count", StringComparison.OrdinalIgnoreCase))
                {
                    value = 0; // For integer OUTPUT parameters
                }
                else if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(string))
                {
                    value = new string(' ', 255); // Reserve space for string OUTPUT parameters
                }
            }

            var param = new LB_DALParam(prop.Name, value, direction);
            dalParams.Add(param);
        }

        return dalParams;
    }

    /// <summary>
    /// Enhanced ExecuteWithOutputAsync that properly handles OUTPUT parameters
    /// </summary>
    public async Task<ExecuteResult> ExecuteWithOutputAsync(string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("ExecuteWithOutput: {StoredProcedure}", storedProcedure);

        await _unitOfWork.EnsureConnectionAsync(cancellationToken);

        var result = new ExecuteResult();
        var dalParams = ConvertToDalParamsWithDirection(parameters);

        try
        {
            // Execute the stored procedure
            result.RowsAffected = await _dal.LB_ExecuteNonQueryAsync(storedProcedure, dalParams, CommandType.StoredProcedure);

            // FIXED: After execution, OUTPUT parameter values are automatically populated by LB_DAL
            // Extract OUTPUT parameter values
            if (dalParams != null)
            {
                foreach (var param in dalParams.Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput))
                {
                    result.OutputValues.Add(new OutputParameter
                    {
                        Name = param.ParameterName.TrimStart('@'),
                        Value = param.Value == DBNull.Value ? null : param.Value
                    });

                    _logger.LogDebug("OUTPUT Parameter {Name}: {Value}", param.ParameterName, param.Value);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure with OUTPUT parameters: {StoredProcedure}", storedProcedure);
            throw;
        }

        return result;
    }












    private List<LB_DALParam>? ConvertToDalParams(object? parameters)
    {
        if (parameters == null) return null;

        var dalParams = new List<LB_DALParam>();

        // Handle Dictionary<string, object>
        if (parameters is Dictionary<string, object> dict)
        {
            foreach (var kvp in dict)
            {
                var value = kvp.Value ?? DBNull.Value;
                dalParams.Add(new LB_DALParam(kvp.Key, value));
            }
            return dalParams;
        }

        // Handle anonymous objects and POCOs
        var properties = parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            var value = prop.GetValue(parameters) ?? DBNull.Value;
            dalParams.Add(new LB_DALParam(prop.Name, value));
        }

        return dalParams;
    }

    private T MapFromReader<T>(DbDataReader reader) where T : class
    {
        var type = typeof(T);
        var instance = Activator.CreateInstance<T>();

        // Simple property mapping using reflection
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < reader.FieldCount; i++)
        {
            var fieldName = reader.GetName(i);

            // Skip TotalCount column used for pagination
            if (fieldName.Equals("TotalCount", StringComparison.OrdinalIgnoreCase))
                continue;

            if (properties.TryGetValue(fieldName, out var property))
            {
                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                if (value != null)
                {
                    // Handle type conversion
                    var convertedValue = ConvertToPropertyType(value, property.PropertyType);
                    property.SetValue(instance, convertedValue);
                }
            }
        }

        return instance;
    }

    private object? ConvertToPropertyType(object value, Type targetType)
    {
        if (value == null || value == DBNull.Value) return null;

        // Handle nullable types
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            targetType = Nullable.GetUnderlyingType(targetType)!;
        }

        // Direct assignment if types match
        if (targetType.IsAssignableFrom(value.GetType()))
            return value;

        // Convert using Convert.ChangeType
        return Convert.ChangeType(value, targetType);
    }

    private object MergeParameters(object? existingParams, object newParams)
    {
        if (existingParams == null) return newParams;

        var merged = new Dictionary<string, object>();

        // Handle existing parameters
        if (existingParams is Dictionary<string, object> existingDict)
        {
            foreach (var kvp in existingDict)
            {
                merged[kvp.Key] = kvp.Value ?? DBNull.Value;
            }
        }
        else
        {
            var existingProps = existingParams.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in existingProps)
            {
                merged[prop.Name] = prop.GetValue(existingParams) ?? DBNull.Value;
            }
        }

        // Handle new parameters
        if (newParams is Dictionary<string, object> newDict)
        {
            foreach (var kvp in newDict)
            {
                merged[kvp.Key] = kvp.Value ?? DBNull.Value;
            }
        }
        else
        {
            var newProps = newParams.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in newProps)
            {
                merged[prop.Name] = prop.GetValue(newParams) ?? DBNull.Value;
            }
        }

        return merged;
    }

    private bool HasColumn(DbDataReader reader, string columnName)
    {
        try
        {
            return reader.GetOrdinal(columnName) >= 0;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    /// <summary>
    /// Convert entities to DataTable for bulk operations
    /// </summary>
    private DataTable ConvertToDataTable<T>(IEnumerable<T> entities) where T : class
    {
        var dataTable = new DataTable();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Create columns
        foreach (var prop in properties)
        {
            var columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            dataTable.Columns.Add(prop.Name, columnType);
        }

        // Add rows
        foreach (var entity in entities)
        {
            var row = dataTable.NewRow();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(entity);
                row[prop.Name] = value ?? DBNull.Value;
            }
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    #endregion

    #region Table Value Parameter Methods

    /// <summary>
    /// Executes stored procedure with table value parameter support using DataTable
    /// This is a simplified version that works with your existing LB DAL
    /// </summary>
    public async Task<BulkOperationResult> ExecuteBulkStoredProcedureAsync(
        string storedProcedureName,
        DataTable tableValueParameter,
        string tableParamName,
        object? additionalParams = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("ExecuteBulkStoredProcedure: {StoredProcedureName}, Records: {RecordCount}",
            storedProcedureName, tableValueParameter.Rows.Count);

        await _unitOfWork.EnsureConnectionAsync(cancellationToken);

        try
        {
            // Create parameters list with table value parameter as DataTable
            var paramList = new List<LB_DALParam>
            {
                new LB_DALParam(tableParamName, tableValueParameter)
            };

            // Add additional parameters
            if (additionalParams != null)
            {
                var additionalDalParams = ConvertToDalParams(additionalParams);
                if (additionalDalParams != null)
                {
                    paramList.AddRange(additionalDalParams);
                }
            }

            // Execute stored procedure - your SP doesn't return a result set, so we use ExecuteNonQuery
            int rowsAffected = await _dal.LB_ExecuteNonQueryAsync(storedProcedureName, paramList, CommandType.StoredProcedure);

            return new BulkOperationResult
            {
                RowsAffected = rowsAffected,
                Status = "SUCCESS",
                Message = $"Successfully processed {rowsAffected} records"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing bulk stored procedure: {StoredProcedureName}", storedProcedureName);
            return new BulkOperationResult
            {
                RowsAffected = 0,
                Status = "ERROR",
                Message = $"Error: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Convert entities to DataTable for table value parameter usage
    /// </summary>
    public DataTable ConvertToTableValueParameter<T>(IEnumerable<T> entities) where T : class
    {
        var dataTable = new DataTable();

        if (!entities.Any())
            return dataTable;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToArray();

        // Create columns based on properties
        foreach (var prop in properties)
        {
            var columnType = GetDataTableColumnType(prop.PropertyType);
            var column = dataTable.Columns.Add(prop.Name, columnType);

            // Allow null for nullable types
            if (IsNullableType(prop.PropertyType))
            {
                column.AllowDBNull = true;
            }
        }

        // Add rows
        foreach (var entity in entities)
        {
            var row = dataTable.NewRow();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(entity);
                row[prop.Name] = value ?? DBNull.Value;
            }

            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    private Type GetDataTableColumnType(Type propertyType)
    {
        // Handle nullable types
        if (IsNullableType(propertyType))
        {
            propertyType = Nullable.GetUnderlyingType(propertyType)!;
        }

        // Map common types to DataTable compatible types
        if (propertyType == typeof(string))
            return typeof(string);
        if (propertyType == typeof(int))
            return typeof(int);
        if (propertyType == typeof(long))
            return typeof(long);
        if (propertyType == typeof(decimal))
            return typeof(decimal);
        if (propertyType == typeof(bool))
            return typeof(bool);
        if (propertyType == typeof(DateTime))
            return typeof(DateTime);
        if (propertyType == typeof(byte))
            return typeof(byte);

        // Default to string for unknown types
        return typeof(string);
    }

    private bool IsNullableType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }




    #endregion
}

