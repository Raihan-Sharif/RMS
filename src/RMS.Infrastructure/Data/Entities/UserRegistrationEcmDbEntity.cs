using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UserRegistrationEcmDbEntity
{
    public string UsrId { get; set; } = null!;

    public DateTime? RegistrationDate { get; set; }

    public DateTime? TrialExpiryDate { get; set; }

    public int? Acknowledgement { get; set; }

    public string? ActivationCode { get; set; }

    public int? ActivateFlag { get; set; }

    public string? UpgradeFlag { get; set; }

    public int? TrialExtendAtmpt { get; set; }

    public int? IsTrialMember { get; set; }

    public DateTime? UpgradeDate { get; set; }

    public string? Cdsno { get; set; }

    public string? ClientCode { get; set; }

    public string? UsrPostCode { get; set; }

    public string? StateCode { get; set; }

    public string? CountryCode { get; set; }

    public string? UsrNationality { get; set; }

    public string? UsrTitle { get; set; }

    public int? OccupCode { get; set; }

    public int? IncGrpCode { get; set; }

    public int? EduLevelCode { get; set; }

    public string? UsrSecretQ { get; set; }

    public string? UsrSecretAns { get; set; }

    public string? UsrPwd { get; set; }

    public string? UsrTrdgPin { get; set; }

    public string? CoCode { get; set; }

    public string? CoBrchCode { get; set; }

    public string? UsrName { get; set; }

    public int? UsrType { get; set; }

    public string? UsrEmail { get; set; }

    public string? UsrPhone { get; set; }

    public string? UsrNicno { get; set; }

    public string? UsrPassNo { get; set; }

    public string? UsrGender { get; set; }

    public DateTime? UsrDob { get; set; }

    public string? UsrAddr { get; set; }

    public string? UsrMobile { get; set; }

    public string? UsrRace { get; set; }
}
