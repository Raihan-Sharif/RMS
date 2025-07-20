using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrJwttoken
{
    public string UsrId { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public string? AccessToken { get; set; }

    public DateTime? LastModified { get; set; }
}
