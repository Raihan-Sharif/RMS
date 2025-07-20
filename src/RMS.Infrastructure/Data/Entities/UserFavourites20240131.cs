using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UserFavourites20240131
{
    public string UserId { get; set; } = null!;

    public int FavGroup { get; set; }

    public string? FavList { get; set; }
}
