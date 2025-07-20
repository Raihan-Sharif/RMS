using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UserRegistration202207211504
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

    public string? SiteId { get; set; }

    public string? UsrSubType { get; set; }

    public string? EmpChk { get; set; }

    public string? EmpName { get; set; }

    public string? EmpType { get; set; }

    public string? EmpYear { get; set; }

    public string? EmpOcc { get; set; }

    public string? EmpAddr { get; set; }

    public string? EmpPost { get; set; }

    public string? EmpPhone { get; set; }

    public string? EmpFax { get; set; }

    public string? EmpBus { get; set; }

    public string? UsrBank { get; set; }

    public string? UsrBankBrch { get; set; }

    public string? UsrBankAcc { get; set; }

    public string? UsrBankAccNm { get; set; }

    public string? SpouseChk { get; set; }

    public string? SpouseName { get; set; }

    public string? SpouseNric { get; set; }

    public string? SpouseEmp { get; set; }

    public string? SpouseEmpAddr { get; set; }

    public string? SpouseOcc { get; set; }

    public string? SpousePhone { get; set; }

    public DateTime? SendActvnDt { get; set; }

    public DateTime? ActvnChgDt { get; set; }

    public DateTime? ActvnExyDt { get; set; }

    public DateTime? DeActvnChgDt { get; set; }
}
