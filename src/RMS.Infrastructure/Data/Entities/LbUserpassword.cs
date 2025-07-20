using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class LbUserpassword
{
    public decimal? Makerid { get; set; }

    public DateTime Actiondate { get; set; }

    public string? Actiontype { get; set; }

    public decimal Userid { get; set; }

    public string? Password { get; set; }
}
