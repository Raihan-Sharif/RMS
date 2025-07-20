using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ResearchDbEntity
{
    public int ResId { get; set; }

    public DateTime ResDate { get; set; }

    public int ResCatId { get; set; }

    public string ResTitle { get; set; } = null!;

    public string ResFileName { get; set; } = null!;

    public string? ResAnalyst { get; set; }

    public int? ResStatus { get; set; }

    public DateTime? PublishDate { get; set; }

    public DateTime? PublishTime { get; set; }

    public string? CountryCode { get; set; }

    public string? StkCode { get; set; }
}
