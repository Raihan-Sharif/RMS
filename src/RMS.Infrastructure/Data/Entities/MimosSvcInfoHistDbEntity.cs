using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MimosSvcInfoHistDbEntity
{
    public string? UsrId { get; set; }

    public string? UsrChannel { get; set; }

    public string? UsrEmail { get; set; }

    public string? UsrMobile { get; set; }

    public string? UpdFlag { get; set; }

    public DateTime? CreationDate { get; set; }
}
