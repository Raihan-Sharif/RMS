using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogFpxresponse
{
    public int SeqNo { get; set; }

    public DateTime? LogDate { get; set; }

    public string? LogUsr { get; set; }

    public string? PaymentRefNo { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? ReturnCode { get; set; }

    public string? ErrMsg { get; set; }
}
