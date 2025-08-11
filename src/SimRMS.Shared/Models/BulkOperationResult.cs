namespace SimRMS.Shared.Models
{
    /// <summary>
    /// Result model for bulk operations with table value parameters
    /// </summary>
    public class BulkOperationResult
    {
        public int RowsAffected { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess => Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase);
    }
}