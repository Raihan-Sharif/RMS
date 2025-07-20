using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MassQuote
{
    public int MassQuoteId { get; set; }

    public DateTime OrderDate { get; set; }
}
