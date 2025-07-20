using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogContractSummary
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string LogAction { get; set; } = null!;

    public string? LogUsr { get; set; }

    public string? BranchCode { get; set; }

    public string? ClientCode { get; set; }

    public string? ContractNo { get; set; }

    public DateTime? ContractDate { get; set; }

    public string Item { get; set; } = null!;

    public string? ExstVal { get; set; }

    public string? NewVal { get; set; }
}
