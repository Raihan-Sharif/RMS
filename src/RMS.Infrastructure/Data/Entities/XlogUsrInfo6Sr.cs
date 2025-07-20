using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogUsrInfo6Sr
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string? BranchId { get; set; }

    public string? UsrId { get; set; }

    public string? UsrSname { get; set; }

    public string? UsrName { get; set; }

    public string? UsrAddr { get; set; }

    public string? UsrNicno { get; set; }

    public string? UsrType { get; set; }

    public string? ShortSellAllow { get; set; }

    public string? DealerCode { get; set; }
}
