using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrderMetadatum
{
    public int OrderSeqNo { get; set; }

    public string? OrderNo { get; set; }

    public string? ClOrdId { get; set; }

    public string? Prefix { get; set; }

    public string? OrderSrc { get; set; }

    public string? SenderType { get; set; }

    public string? FixVer { get; set; }

    public string? LastStatus { get; set; }

    public DateTime? OriginalOrderDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? PrevClOrdId { get; set; }

    public int? OrderQty { get; set; }

    public int? MatchedQty { get; set; }

    public string? SystemOrdId { get; set; }

    public int? SeqNo { get; set; }
}
