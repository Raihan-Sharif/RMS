using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PwdQuestionListDbEntity
{
    public int PwdQuestionId { get; set; }

    public string? PwdQuestion { get; set; }
}
