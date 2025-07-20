using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EnqOrderInQueue
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string BranchCode { get; set; } = null!;

    public string RemisierId { get; set; } = null!;

    public int? NormalOrderDay { get; set; }

    public int? NormalOrderGtd { get; set; }

    public int? OddLotOrderDay { get; set; }

    public int? OddLotOrderGtd { get; set; }
}
