using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntDefList
{
    public int RefNo { get; set; }

    public string? ClntName { get; set; }

    public string? ClntOldIcno { get; set; }

    public string? ClntNewIcno { get; set; }

    public string? ClntPassportNo { get; set; }

    public DateTime? DateDefaulted { get; set; }

    public string? MemCirNoDefaulted { get; set; }

    public DateTime? DateLifted { get; set; }

    public string? MemCirNoLifted { get; set; }

    public decimal? AmtDefaulted { get; set; }

    public int BrkClient { get; set; }
}
