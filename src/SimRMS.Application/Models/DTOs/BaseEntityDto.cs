/// <summary>
/// <para>
/// ===================================================================
/// Title:       Base Entity Model DTO Class
/// Author:      Md. Raihan Sharif
/// Purpose:     This class serves as the base for all entities, providing common properties and metadata.
/// Creation:    25/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
///
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 

namespace SimRMS.Application.Models.DTOs;

public abstract class BaseEntityDto
{
    public string IPAddress { get; set; } = string.Empty; // varchar(39), not nullable

    public int MakerId { get; set; } // int, not nullable

    public DateTime ActionDt { get; set; } // datetime, not nullable

    public DateTime TransDt { get; set; } // date, not nullable

    public byte ActionType { get; set; } // tinyint, not nullable (1: insert, 2: update, 3: delete)

    public int? AuthId { get; set; } // int, nullable

    public DateTime? AuthDt { get; set; } // datetime, nullable

    public DateTime? AuthTransDt { get; set; } // date, nullable

    public byte IsAuth { get; set; } // tinyint, not nullable (0: Unauth, 1: Auth, 2: Denied)

    public byte AuthLevel { get; set; } // tinyint, not nullable (1: First, 2: Second, 3: Third)

    public byte IsDel { get; set; } // tinyint, not nullable (0: Not Deleted, 1: Deleted)

    public string? Remarks { get; set; } // varchar(200), nullable
    public string? MakeBy { get; set; } // Make by Name
    public string? AuthBy { get; set; } // Auth by Name
}

