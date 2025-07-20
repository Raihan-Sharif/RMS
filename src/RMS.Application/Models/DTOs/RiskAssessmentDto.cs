using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Application.Models.DTOs
{
    public class RiskAssessmentDto
    {
        public Guid Id { get; set; }
        public string RiskTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RiskCategory { get; set; } = string.Empty;
        public int ProbabilityScore { get; set; }
        public int ImpactScore { get; set; }
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? MitigationStrategy { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
