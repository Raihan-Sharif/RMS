using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ScreenerCriterionDbEntity
{
    public int CriteriaGrpId { get; set; }

    public int CriteriaId { get; set; }

    public string? CriteriaDesc { get; set; }

    public string? CriteriaShortDesc { get; set; }

    public string? CriteriaActvChartImg { get; set; }

    public string? CriteriaInActvChartImg { get; set; }

    public decimal? CriteriaScale { get; set; }

    public decimal? CriteriaMinVal { get; set; }

    public string? CriteriaValFormat { get; set; }

    public bool? CriteriaShowColumn { get; set; }

    public bool? CriteriaShowSlider { get; set; }

    public string? CriteriaColumnName { get; set; }

    public string? CriteriaPreCondition { get; set; }

    public string? CriteriaImage { get; set; }
}
