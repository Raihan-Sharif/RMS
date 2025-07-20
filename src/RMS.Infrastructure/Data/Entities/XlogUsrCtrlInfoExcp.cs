using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogUsrCtrlInfoExcp
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public int? BranchId { get; set; }

    public string? UsrId { get; set; }

    public int? SuperiorId { get; set; }

    public int? NotifierId { get; set; }

    public int? AssociateId { get; set; }

    public string? AssociatePassword { get; set; }

    public string? Remarks { get; set; }
}
