using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class LbMembershipDbEntity
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

    public decimal? Islockedout { get; set; }

    public decimal? Isfirstlogin { get; set; }

    public DateOnly? Lastlogindate { get; set; }

    public DateOnly? Lastpasswordchangedate { get; set; }

    public DateOnly? Lastlockoutdate { get; set; }

    public decimal? Failedpassatmptcount { get; set; }

    public decimal? Failedpassansatmptcount { get; set; }

    public string? Passwordsalt { get; set; }

    public string? Email { get; set; }

    public decimal? Isloggedin { get; set; }

    public string? Passwordquestion { get; set; }

    public string? Passwordanswer { get; set; }

    public string? Remarks { get; set; }
}
