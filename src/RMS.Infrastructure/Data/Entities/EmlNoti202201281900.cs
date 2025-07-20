using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EmlNoti202201281900
{
    public int? EmlId { get; set; }

    public string? EmlSubject { get; set; }

    public string? EmlDesc { get; set; }

    public string? FromEmlAddr { get; set; }

    public string? CcemailAddr { get; set; }

    public string? BccemailAddr { get; set; }

    public string? Url { get; set; }
}
