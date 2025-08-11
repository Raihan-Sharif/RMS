using SimRMS.Shared.Models;
using System.Data;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       IGenericRepository Interface
/// Author:      Md. Raihan Sharif
/// Purpose:     This interface defines a generic repository pattern with enhanced capabilities for executing SQL queries, stored procedures, and bulk operations.
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 

namespace SimRMS.Domain.Interfaces.Common;

/// <summary>
/// Enhanced Generic repository with SP support and bulk operations
/// </summary>
public interface IGenericRepository
{
    // Query operations (SELECT)
    Task<T?> QuerySingleAsync<T>(string sqlOrSp, object? parameters = null, bool isStoredProcedure = false, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<T>> QueryAsync<T>(string sqlOrSp, object? parameters = null, bool isStoredProcedure = false, CancellationToken cancellationToken = default) where T : class;

    // Enhanced pagination with SP support
    Task<PagedResult<T>> QueryPagedAsync<T>(string sqlOrSp, int pageNumber, int pageSize, object? parameters = null, string? orderBy = null, bool isStoredProcedure = false, CancellationToken cancellationToken = default) where T : class;

    // Command operations (INSERT/UPDATE/DELETE)
    Task<int> ExecuteAsync(string sqlOrSp, object? parameters = null, bool isStoredProcedure = false, CancellationToken cancellationToken = default);
    Task<T> ExecuteScalarAsync<T>(string sqlOrSp, object? parameters = null, bool isStoredProcedure = false, CancellationToken cancellationToken = default);

    // Bulk operations
    Task<int> ExecuteBatchAsync(IEnumerable<(string sql, object? parameters)> commands, CancellationToken cancellationToken = default);

    // SQL Bulk Copy operations
    Task<int> BulkInsertAsync<T>(IEnumerable<T> entities, string tableName, CancellationToken cancellationToken = default) where T : class;
    Task<int> BulkInsertDataTableAsync(DataTable dataTable, string tableName, CancellationToken cancellationToken = default);
    Task<int> BulkInsertFromFileAsync<T>(string filePath, string tableName, Func<string[], T> lineMapper, bool hasHeader = true, string delimiter = ",", CancellationToken cancellationToken = default) where T : class;
}

