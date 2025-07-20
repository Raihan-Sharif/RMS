using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ValRcvdByAutoPaymentResendDbEntity
{
    public DateTime? LogDate { get; set; }

    public string? SClientCode { get; set; }

    public string? SCoBrchCode { get; set; }

    public string? SPymtRefNo { get; set; }

    public string? SContNo { get; set; }

    public string? Remark { get; set; }
}
