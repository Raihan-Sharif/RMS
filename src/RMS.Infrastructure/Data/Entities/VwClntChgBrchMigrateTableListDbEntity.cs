using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClntChgBrchMigrateTableListDbEntity
{
    public string? ColumnName { get; set; }

    public string TableName { get; set; } = null!;
}
