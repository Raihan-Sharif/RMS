using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntNotUpdatedDf10740DbEntity
{
    public DateTime Timestamp { get; set; }

    public string? UsrId { get; set; }

    public string? ClntCode { get; set; }

    public string? CoBrchCode { get; set; }

    public string Remark { get; set; } = null!;
}
