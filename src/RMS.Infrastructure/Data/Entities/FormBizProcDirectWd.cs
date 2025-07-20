using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormBizProcDirectWd
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public int SeqNo { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public string? StkCode { get; set; }

    public int? TrfQty { get; set; }

    public string? TrfQtyWords { get; set; }

    public string? TrfeeCdsno { get; set; }

    public string? TrfeeName { get; set; }

    public string? TrfeeIcno { get; set; }

    public string? TrfeeNationality { get; set; }
}
