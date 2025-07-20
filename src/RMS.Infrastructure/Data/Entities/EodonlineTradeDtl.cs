using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EodonlineTradeDtl
{
    public long? SeqNo { get; set; }

    public string? ClntCode { get; set; }

    public string? AcctCode { get; set; }

    public string? XchgName { get; set; }

    public string? StkSname { get; set; }

    public string? StkCode { get; set; }

    public string? TransType { get; set; }

    public string? Price { get; set; }

    public string? Quantity { get; set; }

    public string? Amount { get; set; }

    public string? OrdSrc { get; set; }

    public string? Currency { get; set; }

    public string? Time { get; set; }

    public string? SequenceNo { get; set; }

    public string? CoBrchCode { get; set; }
}
