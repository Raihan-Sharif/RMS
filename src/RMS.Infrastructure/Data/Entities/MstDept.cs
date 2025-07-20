using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstDept
{
    public string DeptId { get; set; } = null!;

    public string? DeptDesc { get; set; }

    public int? ProdCode { get; set; }

    public int? PolicyCode { get; set; }
}
