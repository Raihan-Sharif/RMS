using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class IntraDayOrdCotrDtl
{
    public DateTime TrxnDate { get; set; }

    public int TrxnSequenceNo { get; set; }

    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public string ContOrderType { get; set; } = null!;

    public DateTime ContOrderDate { get; set; }

    public string ContractNo { get; set; } = null!;

    public int SequenceNo { get; set; }

    public long? CotrQty { get; set; }

    public decimal? CotrPrc { get; set; }

    public decimal? CotrAmt { get; set; }
}
