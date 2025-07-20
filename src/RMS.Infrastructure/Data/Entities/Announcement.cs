using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class Announcement
{
    public int NewsId { get; set; }

    public DateTime? NewsDate { get; set; }

    public string? NewsTitle { get; set; }

    public string? NewsSummary { get; set; }

    public string? NewsFileName { get; set; }

    public string? NewsUser { get; set; }

    /// <summary>
    /// 1-General; 2-Special
    /// </summary>
    public int? NewsType { get; set; }

    /// <summary>
    /// 0-No; 1-Yes
    /// </summary>
    public int? PublishStatus { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public string? AnnouncementControls { get; set; }

    public string? NewsLink { get; set; }

    public string? SysType { get; set; }

    public string? CoBrchCode { get; set; }

    public int? ViewMode { get; set; }
}
