using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogClntInfoExcpArchive
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string? ClntCode { get; set; }

    public string? ClntShortName { get; set; }

    public string? ClntName { get; set; }

    public string? ClntAddress { get; set; }

    public string? PhoneNo { get; set; }

    public int? ClntCdsno { get; set; }

    public string? ClntIcno { get; set; }

    public int? BranchId { get; set; }

    public string? DealerCode { get; set; }

    public string? AccountType { get; set; }

    public string? AllowedCallWarrant { get; set; }

    public string? AllowedClntAssociate { get; set; }

    public string? AllowedShortSelling { get; set; }

    public string? AllowedMultiCurrency { get; set; }

    public string? AllowedMarketMaker { get; set; }

    public string? Remarks { get; set; }
}
