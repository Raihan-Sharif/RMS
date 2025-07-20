using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstDuitNowBankProd
{
    public string BankId { get; set; } = null!;

    public string? BankName { get; set; }

    public string? BankDisplayName { get; set; }

    public string? BankStatus { get; set; }

    public string? BankUrl { get; set; }

    public DateTime? LastUpdateDate { get; set; }

    public DateTime? LastResetDate { get; set; }
}
