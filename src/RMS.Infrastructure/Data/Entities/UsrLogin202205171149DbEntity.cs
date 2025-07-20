using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrLogin202205171149DbEntity
{
    public string UsrId { get; set; } = null!;

    public string? UsrPwd { get; set; }

    public string? UsrPwd1 { get; set; }

    public int? UsrPwdUnscsAtmpt { get; set; }

    public DateTime? UsrPwdLastChgDate { get; set; }

    public DateTime? UsrDisableWrngDate { get; set; }

    public string? UsrTrdgPin { get; set; }

    public int? UsrTrdgPinUnscsAtmpt { get; set; }

    public string? UsrTrdgPinStat { get; set; }

    public DateTime? UsrTrdgPinLastChgDate { get; set; }

    public DateTime? UsrTrdgPinDisableWrngDate { get; set; }

    public int? UsrLogin { get; set; }

    public DateTime? UsrActiveTime { get; set; }

    public DateTime? UsrLastUpdatedDate { get; set; }

    public bool? UsrPwdReset { get; set; }

    public string? UsrActvnCode { get; set; }

    public string? UsrSecretAns1 { get; set; }

    public string? UsrSecretAns2 { get; set; }

    public string? UsrSecretAns3 { get; set; }

    public int? UsrForceLogout { get; set; }

    public string? UsrActvCode { get; set; }

    public DateTime? UsrLastLoginDate { get; set; }

    public bool UsrTrdgPinReset { get; set; }

    public int? UsrOtpresendAtt { get; set; }

    public int? UsrOtpvldtAtt { get; set; }

    public string? UsrOtpcode { get; set; }

    public DateTime? UsrOtpexpiration { get; set; }

    public int? UsrTwoFactorAuth { get; set; }
}
