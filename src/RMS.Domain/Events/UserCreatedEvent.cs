using RMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Domain.Events
{
    public class UserCreatedEvent : IDomainEvent
    {
        public UserCreatedEvent(UsrInfo user)
        {
            User = user;
            OccurredOn = DateTime.UtcNow;
        }

        public UsrInfo User { get; }
        public DateTime OccurredOn { get; }
    }
}
