using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrTypeProdDbEntity
{
    public int UsrType { get; set; }

    public int ProdCode { get; set; }

    public int? ProdPolicy { get; set; }
}
