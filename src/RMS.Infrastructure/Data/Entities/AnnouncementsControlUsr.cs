using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AnnouncementsControlUsr
{
    public int NewsId { get; set; }

    public string UsrId { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;
}
