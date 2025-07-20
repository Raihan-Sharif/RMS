using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstBidLmtDtlDbEntity
{
    public int Id { get; set; }

    public int? BidLmtId { get; set; }

    public decimal? SharePrc { get; set; }

    public decimal? BidLmt { get; set; }

    public int? BidLmtType { get; set; }
}
