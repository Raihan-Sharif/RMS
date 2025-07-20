using RMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Domain.Events
{
    public class RiskAssessmentCreatedEvent : IDomainEvent
    {
        public RiskAssessmentCreatedEvent(RiskAssessment riskAssessment)
        {
            RiskAssessment = riskAssessment;
            OccurredOn = DateTime.UtcNow;
        }

        public RiskAssessment RiskAssessment { get; }
        public DateTime OccurredOn { get; }
    }
}
