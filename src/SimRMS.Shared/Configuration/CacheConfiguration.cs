namespace SimRMS.Shared.Configuration
{
    public class CacheConfiguration
    {
        public const string SectionName = "Cache";

        public int AbsoluteExpirationHours { get; set; } = 1;
        public int SlidingExpirationMinutes { get; set; } = 20;

        public TimeSpan AbsoluteExpiration => TimeSpan.FromHours(AbsoluteExpirationHours);
        public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(SlidingExpirationMinutes);
    }
}