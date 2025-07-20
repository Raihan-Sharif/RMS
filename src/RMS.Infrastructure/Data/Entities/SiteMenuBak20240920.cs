using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class SiteMenuBak20240920
{
    public int NodeSystemType { get; set; }

    public int NodeGrpId { get; set; }

    public int NodeId { get; set; }

    public string? NodeTitle { get; set; }

    public string? NodeTitleVn { get; set; }

    public string? NodeDesc { get; set; }

    public string? NodeDescVn { get; set; }

    public string? NodeUrl { get; set; }

    public string? NodeScript { get; set; }

    public int? NodeParentId { get; set; }

    public int NodeProdCode { get; set; }

    public int? NodePolicy { get; set; }

    public int? NodePolicyView { get; set; }

    public int? NodePolicyAdd { get; set; }

    public int? NodePolicyEdit { get; set; }

    public int? NodePolicyDelete { get; set; }

    public int? NodePolicyApprv { get; set; }

    public bool? NodeMenuShow { get; set; }

    public bool? NodePolicyShow { get; set; }

    public string? LockKey { get; set; }

    public string? NodeActiveIconUrl { get; set; }

    public string? NodeInActiveIconUrl { get; set; }

    public string? BookMarkIconUrl { get; set; }
}
