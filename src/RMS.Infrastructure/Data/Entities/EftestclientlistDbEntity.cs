using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EftestclientlistDbEntity
{
    public string? UserId { get; set; }

    public string? ClientCode { get; set; }

    public string? BranchId { get; set; }

    public string? OriginateUserId { get; set; }

    public string? IsAssociated { get; set; }

    public string? ViewOrder { get; set; }

    public string? PlaceOrder { get; set; }

    public string? ViewClient { get; set; }

    public string? ModifyOrder { get; set; }

    public string? ViewableBranch { get; set; }
}
