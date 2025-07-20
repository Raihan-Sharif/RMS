using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class BannerInfoDbEntity
{
    public int BannerId { get; set; }

    public string? Banner { get; set; }

    public string LinkEnable { get; set; } = null!;

    public string LinkActionType { get; set; } = null!;

    public string AttachmentType { get; set; } = null!;

    public string? AttachmentFileName { get; set; }

    public string? AttachmentUrl { get; set; }

    public DateTime LastUpdated { get; set; }

    public string? BannerHide { get; set; }
}
