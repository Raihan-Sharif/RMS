using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrBookmark
{
    public string UsrId { get; set; } = null!;

    public int NodeSystemType { get; set; }

    public int NodeGrpId { get; set; }

    public int NodeId { get; set; }

    public int SeqNo { get; set; }
}
