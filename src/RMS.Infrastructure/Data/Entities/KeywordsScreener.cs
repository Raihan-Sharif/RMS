using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class KeywordsScreener
{
    public int KeywordId { get; set; }

    public string? TableName { get; set; }

    public int? SystemType { get; set; }

    public int? NodeGrpId { get; set; }

    public int? NodeId { get; set; }

    public string? Keyword { get; set; }

    public string? KeyMapPath { get; set; }

    public string? KeyUrl { get; set; }

    public string? Param { get; set; }
}
