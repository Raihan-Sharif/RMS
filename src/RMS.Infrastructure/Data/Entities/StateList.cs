using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class StateList
{
    public string StateCode { get; set; } = null!;

    public string? StateName { get; set; }
}
