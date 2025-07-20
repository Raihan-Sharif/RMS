using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogClntLimit6SrsHistoryDbEntity
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string? BranchId { get; set; }

    public string? ClntCode { get; set; }

    public string? WithLimitInd { get; set; }

    public decimal? ExceedLimit { get; set; }

    public decimal? MaxBuyCrLimit { get; set; }

    public decimal? MaxSellCrLimit { get; set; }

    public decimal? MaxNettCrLimit { get; set; }

    public decimal? MaxTotalCrLimit { get; set; }

    public string? ClearPrvDayOrd { get; set; }

    public int? TransactionNo { get; set; }
}
