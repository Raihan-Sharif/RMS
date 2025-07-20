using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormCreditLeave
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public string? PatchRelocateTelReq { get; set; }

    public string? Phone1 { get; set; }

    public string? Phone2 { get; set; }

    public string? UsrIdrelief { get; set; }

    public string? AssignClntIntrAcc { get; set; }

    public string? TrxnType { get; set; }
}
