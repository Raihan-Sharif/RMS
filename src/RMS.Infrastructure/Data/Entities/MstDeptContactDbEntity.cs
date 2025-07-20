using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstDeptContactDbEntity
{
    public int Id { get; set; }

    public string DeptId { get; set; } = null!;

    public string ContactName { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string Email { get; set; } = null!;
}
