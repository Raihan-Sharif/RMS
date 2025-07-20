using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstMsgListDbEntity
{
    public string MsgId { get; set; } = null!;

    public string MsgDesc { get; set; } = null!;
}
