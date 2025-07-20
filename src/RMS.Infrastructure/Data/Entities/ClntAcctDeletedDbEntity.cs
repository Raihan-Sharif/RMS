using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntAcctDeletedDbEntity
{
    public string? ClntCode { get; set; }

    public string? CoBrchCode { get; set; }

    public DateTime? DeletedDate { get; set; }
}
