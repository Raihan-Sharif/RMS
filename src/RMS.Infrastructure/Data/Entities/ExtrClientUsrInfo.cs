using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ExtrClientUsrInfo
{
    public string Usrid { get; set; } = null!;

    public string? DlrCode { get; set; }

    public string? CoBrchCode { get; set; }

    public string? UsrName { get; set; }

    public string? UsrType { get; set; }

    public string? UsrNicno { get; set; }

    public string? UsrPassNo { get; set; }

    public string? UsrGender { get; set; }

    public string? UsrRace { get; set; }

    public string? UsrEmail { get; set; }

    public string? UsrPhone { get; set; }

    public string? UsrMobile { get; set; }

    public string? UsrStatus { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public string SiteId { get; set; } = null!;

    public DateTime? UsrLastLoginDate { get; set; }

    public string? UsrPostCode { get; set; }

    public string? StateCode { get; set; }

    public string? CountryCode { get; set; }

    public string? UsrNationality { get; set; }

    public DateTime? TrialExpiryDate { get; set; }
}
