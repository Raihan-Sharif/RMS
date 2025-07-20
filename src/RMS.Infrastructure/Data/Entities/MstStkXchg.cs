using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstStkXchg
{
    public string XchgCode { get; set; } = null!;

    public string? XchgName { get; set; }

    public string? W8benInd { get; set; }

    public string? FrontEndShow { get; set; }

    public string? ImgSrc { get; set; }

    public string? Currency { get; set; }
}
