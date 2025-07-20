using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class Edsvariable
{
    public string VariableName { get; set; } = null!;

    public string? Content { get; set; }
}
