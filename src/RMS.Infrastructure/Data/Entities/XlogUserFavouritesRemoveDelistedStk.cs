using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogUserFavouritesRemoveDelistedStk
{
    public int InsertDate { get; set; }

    public DateTime InsertDateTime { get; set; }

    public string UserId { get; set; } = null!;

    public int FavGroup { get; set; }

    public string OldFavList { get; set; } = null!;

    public string NewFavList { get; set; } = null!;
}
