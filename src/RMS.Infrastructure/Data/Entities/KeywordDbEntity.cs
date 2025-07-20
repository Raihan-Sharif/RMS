using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class KeywordDbEntity
{
    public int KeywordId { get; set; }

    public int? SystemType { get; set; }

    public int? NodeGrpId { get; set; }

    public int? NodeId { get; set; }

    public string? Keyword1 { get; set; }

    public string? KeyMapPath { get; set; }

    public string? KeyUrl { get; set; }
}
