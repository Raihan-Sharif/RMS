using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MassQuoteClOrdIdDbEntity
{
    public int SequenceNo { get; set; }

    public string ClOrdId { get; set; } = null!;

    public string SecondaryClOrdId { get; set; } = null!;
}
