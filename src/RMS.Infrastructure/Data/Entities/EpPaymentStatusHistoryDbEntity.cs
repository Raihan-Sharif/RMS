using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EpPaymentStatusHistoryDbEntity
{
    public DateTime DtCreateDate { get; set; }

    public string SPaymentRefNo { get; set; } = null!;

    public string SContractNo { get; set; } = null!;

    public string STrxnStatus { get; set; } = null!;

    public string STrxnStatusSrc { get; set; } = null!;

    public string? SRemarks { get; set; }

    public int Id { get; set; }
}
