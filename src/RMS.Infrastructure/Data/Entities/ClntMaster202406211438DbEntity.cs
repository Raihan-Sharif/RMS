using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntMaster202406211438DbEntity
{
    public string Gcif { get; set; } = null!;

    public string? ClntName { get; set; }

    public string? ClntNicno { get; set; }

    public string? ClntOicno { get; set; }

    public string? ClntAddr { get; set; }

    public string? ClntPhone { get; set; }

    public string? ClntMobile { get; set; }

    public string? Race { get; set; }

    public string? Gender { get; set; }

    public DateTime? ClntDob { get; set; }

    public int? Nationality { get; set; }

    public string? ClntOffice { get; set; }

    public string? ClntFax { get; set; }

    public bool? Bumiputera { get; set; }

    public string? ClntEmail { get; set; }

    public string CountryCode { get; set; } = null!;
}
