using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormShrMrgnPaymentDbEntity
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public decimal? Ntaamt { get; set; }

    public string? NtarefPno { get; set; }

    public decimal? CotrLossAmt { get; set; }

    public string? CotrLossRefCdno { get; set; }

    public decimal? IntrAmt { get; set; }

    public string? IntrRefCdno { get; set; }
}
