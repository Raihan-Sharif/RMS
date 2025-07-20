using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PwdGenDbEntity
{
    public int SeqNo { get; set; }

    public string UsrId { get; set; } = null!;

    public string? UsrPwd { get; set; }

    public DateTime? UsrPwdChngDate { get; set; }
}
