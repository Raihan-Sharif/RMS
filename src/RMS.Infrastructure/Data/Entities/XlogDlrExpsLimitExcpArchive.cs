using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogDlrExpsLimitExcpArchive
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string BranchId { get; set; } = null!;

    public string DealerId { get; set; } = null!;

    public string? WithLimitInd { get; set; }

    public decimal? ExceedLimit { get; set; }

    public decimal? MaxBuyCrLimit { get; set; }

    public decimal? MaxSellCrLimit { get; set; }

    public decimal? MaxNettCrLimit { get; set; }

    public decimal? MaxTotalCrLimit { get; set; }

    public string? ClearPrvDayOrd { get; set; }

    public int? TransactionNo { get; set; }

    public string? Remarks { get; set; }
}
