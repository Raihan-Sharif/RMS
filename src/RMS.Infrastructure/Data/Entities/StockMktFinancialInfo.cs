using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class StockMktFinancialInfo
{
    public string Type { get; set; } = null!;

    public string Sector { get; set; } = null!;

    public string? Data { get; set; }
}
