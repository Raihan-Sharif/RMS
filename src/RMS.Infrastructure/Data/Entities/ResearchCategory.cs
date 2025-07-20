using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ResearchCategory
{
    public long CategoryId { get; set; }

    public string CategoryCode { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public long CategoryParent { get; set; }
}
