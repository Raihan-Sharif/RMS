using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClientListDbEntity
{
    public string UserId { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public string OriginateUserId { get; set; } = null!;

    public int? IsAssociated { get; set; }
}
