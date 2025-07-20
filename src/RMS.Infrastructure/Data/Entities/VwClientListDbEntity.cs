using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClientListDbEntity
{
    public string? UsrId { get; set; }

    public string ClntCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string? OriginateId { get; set; }

    public int? IsAssociated { get; set; }

    public int? ViewOrder { get; set; }

    public int? PlaceOrder { get; set; }

    public int? ViewClient { get; set; }
}
