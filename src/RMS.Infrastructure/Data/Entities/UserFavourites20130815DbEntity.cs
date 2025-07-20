using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UserFavourites20130815DbEntity
{
    public string UserId { get; set; } = null!;

    public int FavGroup { get; set; }

    public string FavList { get; set; } = null!;
}
