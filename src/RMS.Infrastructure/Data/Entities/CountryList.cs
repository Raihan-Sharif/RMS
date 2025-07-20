using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class CountryList
{
    public string CountryCode { get; set; } = null!;

    public string? CountryName { get; set; }
}
