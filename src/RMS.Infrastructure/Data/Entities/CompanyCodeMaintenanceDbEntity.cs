using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class CompanyCodeMaintenanceDbEntity
{
    public string CompanyCode { get; set; } = null!;

    public string? ServerName { get; set; }

    public string? DatabaseName { get; set; }

    public string? Dbdescription { get; set; }

    public string? Email { get; set; }
}
