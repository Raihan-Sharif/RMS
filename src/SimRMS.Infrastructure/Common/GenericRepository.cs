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
    /// Handle pagination for Stored Procedures
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

        var dalParams = ConvertToDalParams(spParameters);

        // Set TotalCount as output parameter
        var totalCountParam = dalParams?.FirstOrDefault(p => p.ParameterName == "TotalCount");
        if (totalCountParam != null)
        {
            totalCountParam.Direction = ParameterDirection.Output;
        }

        var results = new List<T>();
        int totalCount = 0;

        using var reader = await _dal.LB_GetDbDataReaderAsync(storedProcedureName, dalParams, CommandType.StoredProcedure);

        // Read data from first result set
        while (await reader.ReadAsync())
        {
            // Try to get total count from column (if SP includes it)
            if (totalCount == 0 && HasColumn(reader, "TotalCount"))
            {
                totalCount = reader.GetInt32("TotalCount");
            }

            results.Add(MapFromReader<T>(reader));
        }

        // If SP has second result set with just total count
        if (totalCount == 0 && await reader.NextResultAsync())
        {
            if (await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(0); // First column should be count
            }
        }

        // If SP uses output parameter for total count
        if (totalCount == 0 && totalCountParam != null)
        {
            totalCount = Convert.ToInt32(totalCountParam.Value);
        }

        return new PagedResult<T>
        {
            Data = results,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
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

        // Add existing parameters
        var existingProps = existingParams.GetType().GetProperties();
        foreach (var prop in existingProps)
        {
            merged[prop.Name] = prop.GetValue(existingParams) ?? DBNull.Value;
        }

        // Add new parameters (overwrite if duplicate)
        var newProps = newParams.GetType().GetProperties();
        foreach (var prop in newProps)
        {
            merged[prop.Name] = prop.GetValue(newParams) ?? DBNull.Value;
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
}