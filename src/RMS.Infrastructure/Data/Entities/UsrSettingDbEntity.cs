using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrSettingDbEntity
{
    public int SetId { get; set; }

    public string? SetDesc { get; set; }

    public string? SetValue { get; set; }
}
