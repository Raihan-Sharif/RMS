using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class LbUserDbEntity
{
    public decimal? Makerid { get; set; }

    public DateOnly? Actiondate { get; set; }

    public DateOnly? Transdate { get; set; }

    public string? Actiontype { get; set; }

    public decimal? Authid { get; set; }

    public DateOnly? Authdate { get; set; }

    public DateOnly? Authtransdate { get; set; }

    public decimal? Isauth { get; set; }

    public decimal? Authlevel { get; set; }

    public decimal? Isdel { get; set; }

    public decimal Userid { get; set; }

    public string? Username { get; set; }

    public string? Remarks { get; set; }
}
