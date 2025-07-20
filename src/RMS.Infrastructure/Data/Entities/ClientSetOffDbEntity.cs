using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClientSetOffDbEntity
{
    public string ClientCode { get; set; } = null!;

    public DateTime TransDate { get; set; }

    public decimal SetOffAmount { get; set; }

    public string SetOffType { get; set; } = null!;

    public DateTime TransDueDate { get; set; }

    public decimal InterestAmount { get; set; }

    public string InterestType { get; set; } = null!;
}
