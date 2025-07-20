using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrCoBrchAssignDbEntity
{
    public string UsrId { get; set; } = null!;

    public string CoCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;
}
