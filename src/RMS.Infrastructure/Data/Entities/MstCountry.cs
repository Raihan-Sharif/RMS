using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstCountry
{
    public string CountryCode { get; set; } = null!;

    public string? CountryName { get; set; }
}
