using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FsttradeInfoDbEntity
{
    public string XchgCode { get; set; } = null!;

    public int TransCode { get; set; }

    public decimal TransSeqNo { get; set; }

    public string? ClntNo { get; set; }

    public int? DealerId { get; set; }

    public int? BranchId { get; set; }

    public int? TerminalId { get; set; }

    public int? OrderNo { get; set; }

    public int? ContDate { get; set; }

    public int? Trsno { get; set; }

    public string? StkCode { get; set; }

    public int? SettCode { get; set; }

    public int? BuySellCode { get; set; }

    public long? MatchQty { get; set; }

    public int? OrderType { get; set; }

    public decimal? MatchedPrice { get; set; }

    public int? MatchedTime { get; set; }

    public int? CounterBrokerId { get; set; }

    public decimal? ClntCdsno { get; set; }

    public string? OldClntNo { get; set; }

    public int? AmendTime { get; set; }

    public int? UsrId { get; set; }

    public int? ActionCode { get; set; }

    public long? QtyBefore { get; set; }

    public long? OrdQty { get; set; }

    public decimal? OrdPrice { get; set; }

    public int? OrdDate { get; set; }

    public int? OrdTime { get; set; }

    public int? UpdateInd { get; set; }

    public decimal? DefaulterRefNum { get; set; }

    public string? ClntName { get; set; }

    public string? ClientOldIcno { get; set; }

    public string? ClientIcno { get; set; }

    public string? ClientPassNo { get; set; }

    public int? DateDefaulted { get; set; }

    public string? MemCirNoDefaulted { get; set; }

    public int? DateLifted { get; set; }

    public string? MemCirNoLifted { get; set; }

    public decimal? AmtDefaulted { get; set; }

    public int? BatchControl { get; set; }

    public int? BosmsgCode { get; set; }

    public int? OrderPlacedSeqNo { get; set; }

    public DateTime? LastUpdateDate { get; set; }

    public int Mbmssend { get; set; }

    public DateTime? MbmssentTime { get; set; }

    public decimal? ProgramId { get; set; }
}
