using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntAcct202406211438DbEntity
{
    public string ClntCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string? Gcif { get; set; }

    public string? ClntSname { get; set; }

    public string? ClntStat { get; set; }

    public string? ClntTrdgStat { get; set; }

    public string? ClntAcctType { get; set; }

    public string? ClntCdsno { get; set; }

    public string? ClntDlrCode { get; set; }

    public bool? ClntAllowAssociate { get; set; }

    public bool? ClntCrossAmend { get; set; }

    public bool? ClntDlrReassign { get; set; }

    public string? ClntReassignDlrCode { get; set; }

    public DateTime? ClntOpenDate { get; set; }

    public string? ClntOpenBy { get; set; }

    public DateTime? ClntCloseDate { get; set; }

    public string? ClntCloseBy { get; set; }

    public DateTime? ClntLastUpdatedDate { get; set; }

    public string? ClntLastUpdatedBy { get; set; }

    public DateTime? ClntActvnDate { get; set; }

    public DateTime? ClntReActvnDate { get; set; }

    public DateTime? ClntSuspendDate { get; set; }

    public string? ClntSuspendRemarks { get; set; }

    public DateTime? ClntDormantDate { get; set; }

    public string? ClntLegalStatus { get; set; }

    public string? ClntBank { get; set; }

    public string? ClntInternalAcType { get; set; }

    public string? ClntMrgnAcNo { get; set; }

    public decimal? ClredTabal { get; set; }

    public decimal? EarmarkAmt { get; set; }

    public decimal? TrustAmt { get; set; }

    public decimal? ApprvTrdgLimit { get; set; }

    public decimal? AvailableLimit { get; set; }

    public bool? ClntDormantStat { get; set; }

    public bool? ClntDelinquentStat { get; set; }

    public int? ClntActvAlert { get; set; }

    public string? ClntRemId { get; set; }

    public decimal? CotrLossAmt { get; set; }

    public DateTime? ClntLastTrxDate { get; set; }

    public string? ClntBankAcct { get; set; }

    public int ClntDefStat { get; set; }

    public string? ClntActvCode { get; set; }

    public DateTime? Up2Ecosdate { get; set; }

    public string? ClntName { get; set; }

    public DateTime? ShrCheckRegDate { get; set; }

    public DateTime? W8benExpiryDate { get; set; }

    public string? W8benInd { get; set; }

    public string? OutboundInd { get; set; }

    public string? On9ClntInd { get; set; }

    public string? MultiCcyInd { get; set; }

    public string ClntType { get; set; } = null!;

    public string? ClntMainType { get; set; }

    public string AllowTrustWithdrawal { get; set; } = null!;

    public int? RegInfoVldtAtt { get; set; }

    public int? RegOtpresendAtt { get; set; }

    public int? RegOtpvldtAtt { get; set; }

    public string? RegOtpcode { get; set; }

    public DateTime? Otpexpiration { get; set; }

    public int? RegEmailCount { get; set; }
}
