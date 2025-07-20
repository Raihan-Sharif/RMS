using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstClntXexp
{
    public bool? ClntExpsCheckFst { get; set; }

    public decimal? ClntExpsFstamt { get; set; }

    public decimal? ClntExpsEcosamt { get; set; }
}
