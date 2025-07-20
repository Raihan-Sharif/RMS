using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class BroadcastMsgMaint
{
    public int NewsId { get; set; }

    public DateTime? NewsDate { get; set; }

    public string? NewsTitle { get; set; }

    public string? NewsSummary { get; set; }

    public string? NewsFileName { get; set; }

    public string? NewsUser { get; set; }

    /// <summary>
    /// 0-No; 1-Yes
    /// </summary>
    public int? PublishStatus { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public string? Channel { get; set; }

    public string? MobileMsg { get; set; }

    public int? MobileNotifyFlag { get; set; }
}
