using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstClntNettLmtExpDbEntity
{
    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public decimal? ClntExpsNetAmt { get; set; }

    public string? ClntDlrCode { get; set; }

    public decimal? DlrExpsNetAmt { get; set; }

    public DateTime UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
