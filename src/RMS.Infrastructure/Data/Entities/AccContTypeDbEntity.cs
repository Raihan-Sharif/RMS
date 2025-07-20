using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AccContTypeDbEntity
{
    public string SContType { get; set; } = null!;

    public string? SContTitle { get; set; }

    public string? SContDesc { get; set; }

    public string? SContCat { get; set; }

    public int? IContTypeSeq { get; set; }

    public byte? IPayable { get; set; }

    public byte? IViewable { get; set; }
}
