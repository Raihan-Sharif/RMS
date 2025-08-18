/// <summary>
/// <para>
/// ===================================================================
/// Title:       Execute Result Models
/// Author:      Md. Raihan Sharif
/// Purpose:     Result models for database execution operations with output parameters
/// Creation:    17/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Shared.Models;

/// <summary>
/// Result of executing stored procedures with output parameters
/// </summary>
public class ExecuteResult
{
    public int RowsAffected { get; set; }
    public List<OutputParameter> OutputValues { get; set; } = new List<OutputParameter>();

    public T? GetOutputValue<T>(string parameterName)
    {
        var param = OutputValues.FirstOrDefault(p => p.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
        if (param?.Value == null) return default(T);
        
        try
        {
            return (T)Convert.ChangeType(param.Value, typeof(T));
        }
        catch
        {
            return default(T);
        }
    }
}

/// <summary>
/// Represents an output parameter from stored procedure execution
/// </summary>
public class OutputParameter
{
    public string Name { get; set; } = string.Empty;
    public object? Value { get; set; }
}