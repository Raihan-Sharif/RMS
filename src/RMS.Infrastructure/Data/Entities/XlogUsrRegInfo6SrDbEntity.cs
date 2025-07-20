using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogUsrRegInfo6SrDbEntity
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string? BranchId { get; set; }

    public string? UsrId { get; set; }

    public string? Sex { get; set; }

    public string? Race { get; set; }

    public string? TelNo { get; set; }

    public string? Dob { get; set; }

    public string? DateTdr { get; set; }

    public string? DateDealer { get; set; }

    public string? DateResigned { get; set; }

    public string? Qualification { get; set; }
}
