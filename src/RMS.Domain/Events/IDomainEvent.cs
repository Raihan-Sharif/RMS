using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace RMS.Domain.Events
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}
