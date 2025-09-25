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

namespace SimRMS.Infrastructure.Common;


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

    #region Query Operations (SELECT)

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
    /// FULLY CORRECTED: Enhanced pagination supporting both inline SQL and Stored Procedures with OUTPUT parameters
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

    #endregion

    #region Command Operations (INSERT/UPDATE/DELETE)

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

    /// <summary>
    /// FULLY CORRECTED: Enhanced ExecuteWithOutputAsync that properly handles multiple OUTPUT parameters
    /// Uses LB_ExecuteNonQueryAsync which automatically populates OUTPUT parameters in LB_DAL framework
    /// </summary>
    public async Task<ExecuteResult> ExecuteWithOutputAsync(string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("ExecuteWithOutput: {StoredProcedure}", storedProcedure);

        await _unitOfWork.EnsureConnectionAsync(cancellationToken);

        var result = new ExecuteResult();
        var dalParams = ConvertToDalParamsWithDirection(parameters);

        try
        {
            // CRITICAL: Use LB_ExecuteNonQueryAsync which automatically handles OUTPUT parameters
            // The LB_DAL framework populates OUTPUT parameter values after execution
            var affectedRows = await _dal.LB_ExecuteNonQueryAsync(storedProcedure, dalParams, CommandType.StoredProcedure);

            // CRITICAL: After LB_ExecuteNonQueryAsync, OUTPUT parameters are automatically populated by LB_DAL
            if (dalParams != null)
            {
                foreach (var param in dalParams.Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput))
                {
                    var outputParam = new OutputParameter
                    {
                        Name = param.ParameterName.TrimStart('@'),
                        Value = param.Value == DBNull.Value ? null : param.Value
                    };

                    result.OutputValues.Add(outputParam);

                    _logger.LogDebug("OUTPUT Parameter {Name}: {Value}", param.ParameterName, param.Value);
                }
            }

            // FIXED: Get RowsAffected from OUTPUT parameter, not from DataReader count
            var rowsAffectedParam = result.OutputValues.FirstOrDefault(p =>
                p.Name.Equals("RowsAffected", StringComparison.OrdinalIgnoreCase));

            if (rowsAffectedParam?.Value != null)
            {
                result.RowsAffected = Convert.ToInt32(rowsAffectedParam.Value);
            }
            else
            {
                // Fallback to the return value from ExecuteNonQuery (though it may not be accurate for SPs)
                result.RowsAffected = affectedRows;
            }

            _logger.LogDebug("Stored procedure executed successfully. RowsAffected: {RowsAffected}, OutputParams: {OutputCount}",
                result.RowsAffected, result.OutputValues.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure with OUTPUT parameters: {StoredProcedure}", storedProcedure);
            throw;
        }

        return result;
    }

    #endregion

    #region Private Helper Methods - Pagination

    /// <summary>
    /// FULLY CORRECTED: Handle pagination for Stored Procedures with proper OUTPUT parameter support
    /// Uses a hybrid approach: Execute to get OUTPUT params, then DataReader for result sets
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

        try
        {
            // CRITICAL SOLUTION: For pagination SPs that return data AND have OUTPUT params,
            // we need to execute TWICE because LB_DAL has limitations:
            // 1. First execution with LB_ExecuteNonQueryAsync to get OUTPUT parameters
            // 2. Second execution with LB_GetDbDataReaderAsync to get result set data

            // STEP 1: Execute to populate OUTPUT parameters
            await _dal.LB_ExecuteNonQueryAsync(storedProcedureName, dalParams, CommandType.StoredProcedure);

            // Extract OUTPUT parameter values (now populated by LB_DAL)
            if (dalParams != null)
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

            // STEP 2: Execute again to get result set data
            // Reset parameters to INPUT only (exclude OUTPUT params for DataReader call)
            var inputParams = ConvertToDalParams(parameters);
            var readerParams = MergeParameters(parameters, new { PageNumber = pageNumber, PageSize = pageSize });
            var readerDalParams = ConvertToDalParams(readerParams);

            using var reader = await _dal.LB_GetDbDataReaderAsync(storedProcedureName, readerDalParams, CommandType.StoredProcedure);

            // Read data from result set
            while (await reader.ReadAsync())
            {
                results.Add(MapFromReader<T>(reader));
            }

            // FALLBACK: If OUTPUT parameter didn't work, try getting TotalCount from second result set
            if (totalCount == 0 && await reader.NextResultAsync())
            {
                if (await reader.ReadAsync())
                {
                    // Try to get total count from column (SP includes it in 2nd result set)
                    if (HasColumn(reader, "TotalCount"))
                    {
                        totalCount = reader.GetInt32("TotalCount");
                    }
                    else
                    {
                        totalCount = reader.GetInt32(0); // First column should be count
                    }

                    _logger.LogDebug("Retrieved TotalCount from second result set: {TotalCount}", totalCount);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing paginated stored procedure: {StoredProcedure}", storedProcedureName);
            throw;
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

    #endregion

    #region Private Helper Methods - Parameter Handling

    /// <summary>
    /// FULLY CORRECTED: Enhanced parameter detection that includes your specific OUTPUT parameter names
    /// </summary>
    private bool IsOutputParameter(string parameterName)
    {
        // Enhanced OUTPUT parameter naming patterns including your specific ones
        var outputPatterns = new[] {
            "RowsAffected", "StatusCode", "StatusMsg", "TotalCount", "RecordCount",
            "Result", "ReturnValue", "Count", "InsertedCode", "InsertedId",
            "GeneratedCode", "OutputCode", "NewId", "NewCode", "ErrorCode", "ErrorMsg","ErrorMessage", "NewGroupCode"
        };

        return outputPatterns.Any(pattern =>
            parameterName.Equals(pattern, StringComparison.OrdinalIgnoreCase) ||
            parameterName.EndsWith(pattern, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// FULLY CORRECTED: Enhanced parameter conversion with proper size allocation for string OUTPUT parameters
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

                // FIXED: Proper size allocation for OUTPUT parameters
                if (direction == ParameterDirection.Output)
                {
                    if (kvp.Key.Contains("Count", StringComparison.OrdinalIgnoreCase) ||
                        kvp.Key.Contains("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        value = 0; // For integer OUTPUT parameters
                    }
                    else if (kvp.Key.Contains("Code", StringComparison.OrdinalIgnoreCase))
                    {
                        value = new string(' ', 10); // Reserve space for code fields (usually 6-10 chars)
                    }
                    else if (kvp.Key.Contains("Msg", StringComparison.OrdinalIgnoreCase) ||
                            kvp.Key.Contains("Message", StringComparison.OrdinalIgnoreCase))
                    {
                        value = new string(' ', 4000); // Reserve space for message fields (match SP declaration)
                    }
                    else
                    {
                        value = new string(' ', 255); // Default string OUTPUT parameter size
                    }
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

            // FIXED: Proper size allocation for OUTPUT parameters based on property type
            if (direction == ParameterDirection.Output)
            {
                if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                {
                    value = 0;
                }
                else if (prop.PropertyType == typeof(string))
                {
                    if (prop.Name.Contains("Code", StringComparison.OrdinalIgnoreCase))
                    {
                        value = new string(' ', 10); // Code fields
                    }
                    else if (prop.Name.Contains("Msg", StringComparison.OrdinalIgnoreCase) ||
                            prop.Name.Contains("Message", StringComparison.OrdinalIgnoreCase))
                    {
                        value = new string(' ', 4000); // Message fields (match SP declaration)
                    }
                    else
                    {
                        value = new string(' ', 255); // Default string size
                    }
                }
            }

            dalParams.Add(new LB_DALParam(prop.Name, value, direction));
        }

        return dalParams;
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
                dalParams.Add(new LB_DALParam(kvp.Key, kvp.Value ?? DBNull.Value));
            }
            return dalParams;
        }

        // Handle anonymous objects and POCOs
        var properties = parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            dalParams.Add(new LB_DALParam(prop.Name, prop.GetValue(parameters) ?? DBNull.Value));
        }

        return dalParams;
    }

    private bool HasOutputParameters(object? parameters)
    {
        if (parameters == null) return false;

        var properties = parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        return properties.Any(p => IsOutputParameter(p.Name));
    }

    private object MergeParameters(object? original, object additional)
    {
        if (original == null) return additional;

        var dict = new Dictionary<string, object>();

        // Add original parameters
        if (original is Dictionary<string, object> originalDict)
        {
            foreach (var kvp in originalDict)
            {
                dict[kvp.Key] = kvp.Value;
            }
        }
        else
        {
            var originalProps = original.GetType().GetProperties();
            foreach (var prop in originalProps)
            {
                dict[prop.Name] = prop.GetValue(original) ?? DBNull.Value;
            }
        }

        // Add additional parameters
        var additionalProps = additional.GetType().GetProperties();
        foreach (var prop in additionalProps)
        {
            dict[prop.Name] = prop.GetValue(additional) ?? DBNull.Value;
        }

        return dict;
    }

    private bool HasColumn(DbDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
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

    private T MapFromReader<T>(DbDataReader reader) where T : class
    {
        var type = typeof(T);
        var instance = Activator.CreateInstance<T>();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (!property.CanWrite) continue;

            try
            {
                var columnName = property.Name;
                if (HasColumn(reader, columnName))
                {
                    var value = reader[columnName];
                    if (value != DBNull.Value)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(instance, value.ToString()?.Trim());
                        }
                        else
                        {
                            // Handle nullable types properly
                            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            var convertedValue = Convert.ChangeType(value, targetType);
                            property.SetValue(instance, convertedValue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to map property {PropertyName}: {Error}", property.Name, ex.Message);
            }
        }

        return instance;
    }

    #endregion

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