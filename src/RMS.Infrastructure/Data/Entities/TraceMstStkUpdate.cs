using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TraceMstStkUpdate
{
    public long Id { get; set; }

    public string? XchgCode { get; set; }

    public string? StkCode { get; set; }

    public decimal? StkLastDonePrice { get; set; }

    public decimal? StkRefPrice { get; set; }

    public int? ClntHostProcessId { get; set; }

    public string? ClntHostName { get; set; }

    public string? ClntAppName { get; set; }

    public string? ClntIpaddr { get; set; }

    public int? RecInsertedDate { get; set; }

    public DateTime? RecInsertedDateTime { get; set; }
}
