using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ValRcvdBySpupdPymtContDbEntity
{
    public DateTime? DtCreateDate { get; set; }

    public string? SPymtRefNo { get; set; }

    public string? SPymtStatus { get; set; }

    public string? SLocationCode { get; set; }

    public string? SRemark { get; set; }
}
