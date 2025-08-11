/// <summary>
/// <para>
/// ===================================================================
/// Title:       User Statistics DTO
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents the data transfer object for user statistics.
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

namespace SimRMS.Application.Models.DTOs
{
    public class UserStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int SuspendedUsers { get; set; }
        public int ClosedUsers { get; set; }
        public List<RmsTypeStatistic> RmsTypes { get; set; } = new();
        public List<CompanyCodeStatistic> CompanyCodes { get; set; } = new();
    }

    public class RmsTypeStatistic
    {
        public string RmsType { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class CompanyCodeStatistic
    {
        public string CoCode { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}