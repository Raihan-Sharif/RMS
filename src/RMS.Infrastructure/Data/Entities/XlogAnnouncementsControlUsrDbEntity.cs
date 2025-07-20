using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogAnnouncementsControlUsrDbEntity
{
    public int? NewsId { get; set; }

    public string? UsrId { get; set; }

    public string? CoBrchCode { get; set; }

    public int SeqNo { get; set; }

    public string? LogUsr { get; set; }

    public DateTime? LogDate { get; set; }

    /// <summary>
    /// A-Add; D-Delete
    /// </summary>
    public string LogAction { get; set; } = null!;
}
