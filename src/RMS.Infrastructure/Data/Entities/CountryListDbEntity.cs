using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class CountryListDbEntity
{
    public string CountryCode { get; set; } = null!;

    public string? CountryName { get; set; }
}
