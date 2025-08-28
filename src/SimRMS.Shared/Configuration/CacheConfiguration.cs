/// <summary>
/// <para>
/// ===================================================================
/// Title:       Cache Service
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents the configuration settings(appsettings) for caching, including expiration policies.
/// Creation:    28/Aug/2025
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

namespace SimRMS.Shared.Configuration;

    public class CacheConfiguration
    {
        public const string SectionName = "Cache";

        public int AbsoluteExpirationHours { get; set; } = 1;
        public int SlidingExpirationMinutes { get; set; } = 20;

        public TimeSpan AbsoluteExpiration => TimeSpan.FromHours(AbsoluteExpirationHours);
        public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(SlidingExpirationMinutes);
    }
