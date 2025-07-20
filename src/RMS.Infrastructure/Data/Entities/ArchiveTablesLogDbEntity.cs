using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ArchiveTablesLogDbEntity
{
    public int ArchiveDate { get; set; }

    public DateTime ArchiveDateTime { get; set; }

    public string DatabaseName { get; set; } = null!;

    public string ArchiveDatabaseName { get; set; } = null!;

    public string TableName { get; set; } = null!;

    public string ArchiveTableName { get; set; } = null!;

    public int DaysToKeep { get; set; }

    public DateTime ArchiveStartTime { get; set; }

    public DateTime ArchiveEndTime { get; set; }

    public int TotalInsertedRecord { get; set; }

    public int TotalDeletedRecord { get; set; }

    public string ArchiveStatus { get; set; } = null!;

    public string? ErrMsg { get; set; }
}
