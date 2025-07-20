using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogUsrTypeList
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string LogAction { get; set; } = null!;

    public string LogUsr { get; set; } = null!;

    public int? UsrType { get; set; }

    public string Item { get; set; } = null!;

    public string? ExstVal { get; set; }

    public string? NewVal { get; set; }

    public int? ProdCode { get; set; }

    public string? ExstMenuItem { get; set; }

    public string? NewMenuItem { get; set; }

    public int Id { get; set; }
}
