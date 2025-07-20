using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ValRcvdBySpupdPymtRefNoDbEntity
{
    public DateTime? DtCreateDate { get; set; }

    public string? SMerchantId { get; set; }

    public string? SChannelId { get; set; }

    public string? SBillAccNo { get; set; }

    public string? SPymtRefNo { get; set; }

    public decimal? DRetPymtByBank { get; set; }

    public string? SRetBankRefNo { get; set; }

    public string? SRetTransDate { get; set; }

    public string? SRetTransTime { get; set; }

    public string? SRetUsrName { get; set; }

    public string? SRetTransCode { get; set; }

    public string? SSptype { get; set; }

    public string? SLocationCode { get; set; }

    public string? SRemark { get; set; }
}
