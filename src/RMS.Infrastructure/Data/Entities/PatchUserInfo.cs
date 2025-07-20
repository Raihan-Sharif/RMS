using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PatchUserInfo
{
    public string UsrId { get; set; } = null!;

    public string UsrName { get; set; } = null!;

    public string PatchUsrName { get; set; } = null!;
}
