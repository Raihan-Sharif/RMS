using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ConfirmExTrsnoDbEntity
{
    public int SequenceNo { get; set; }

    public string Trsno { get; set; } = null!;

    public string OrderType { get; set; } = null!;
}
