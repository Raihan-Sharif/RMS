using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MassQuoteDbEntity
{
    public int MassQuoteId { get; set; }

    public DateTime OrderDate { get; set; }
}
