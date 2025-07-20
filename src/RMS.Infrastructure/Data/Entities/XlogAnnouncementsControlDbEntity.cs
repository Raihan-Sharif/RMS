using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogAnnouncementsControlDbEntity
{
    public int? NewsId { get; set; }

    public int? UsrType { get; set; }

    public int SeqNo { get; set; }

    public string LogUsr { get; set; } = null!;

    public DateTime LogDate { get; set; }

    /// <summary>
    /// A-Add; D-Delete
    /// </summary>
    public string LogAction { get; set; } = null!;
}
