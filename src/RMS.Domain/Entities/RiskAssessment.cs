using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Domain.Entities
{
    public class RiskAssessment : BaseEntity
    {
        public string RiskTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RiskCategory { get; set; } = string.Empty;
        public int ProbabilityScore { get; set; }
        public int ImpactScore { get; set; }
        public int RiskScore => ProbabilityScore * ImpactScore;
        public string RiskLevel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? MitigationStrategy { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
    }
}
