using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ItrFileTrnxDbEntity
{
    public int FileId { get; set; }

    public string FileName { get; set; } = null!;

    public string OriFileName { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public string Operation { get; set; } = null!;

    public DateTime? LastUpdatedDate { get; set; }
}
