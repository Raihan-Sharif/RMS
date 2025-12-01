using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRMS.Application.Models.DTOs
{
    public class ExchangeBrokerDTO
    {
        public int XchgPrefix { get; set; }
        public string BrokerCode { get; set; } = string.Empty;
        public string BrokerCodeWithXchgPrefix { get; set; } = string.Empty;
    }
}
