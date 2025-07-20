using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntOdrDate
{
    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public DateTime? LastTradingDateOn { get; set; }

    public DateTime? LastTradingDateOff { get; set; }
}
