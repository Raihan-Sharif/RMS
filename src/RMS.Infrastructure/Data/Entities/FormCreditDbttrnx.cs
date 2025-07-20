using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormCreditDbttrnx
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public string? TrnxType { get; set; }

    public string? BrkName { get; set; }

    public string? CtrPtyDealer { get; set; }

    public string? TelNo { get; set; }

    public string? FaxNo { get; set; }

    public string? StkCode { get; set; }

    public int? Qty { get; set; }

    public decimal? Price { get; set; }

    public decimal? ApprvTrdgLimit { get; set; }

    public decimal? AvailableLimit { get; set; }
}
