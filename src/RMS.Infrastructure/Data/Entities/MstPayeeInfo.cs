using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstPayeeInfo
{
    public string PaymentBank { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string? PayeeCode { get; set; }

    public string? UsrId { get; set; }

    public string? UsrPwd { get; set; }
}
