using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Application.Interfaces
{
    public interface IHandshakeService
    {
        Task<bool> PerformHandshakeAsync();
        Task<bool> IsTokenServiceAvailableAsync();
        bool IsHandshakeCompleted { get; }
        DateTime? LastHandshakeTime { get; }
    }
}
