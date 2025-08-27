/// <summary>
/// <para>
/// ===================================================================
/// Title:       Auth Level Enumeration
/// Author:      Md. Raihan Sharif
/// Purpose:     Defines authorization levels for system access
/// Creation:    27/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Shared.Constants;

/// <summary>
/// Represents different authorization levels in the system
/// </summary>
public enum AuthLevelEnum : byte
{
    /// <summary>
    /// No authorization level
    /// </summary>
    None = 0,

    /// <summary>
    /// Level 1 initial to authorization
    /// </summary>
    Level1 = 1,

    /// <summary>
    /// Level 2 authorization
    /// </summary>
    Level2 = 2,

    ///// <summary>
    ///// Level 3 authorization
    ///// </summary>
    //Level3 = 3,

    ///// <summary>
    ///// Level 4 authorization
    ///// </summary>
    //Level4 = 4,

    ///// <summary>
    ///// Level 5 authorization
    ///// </summary>
    //Level5 = 5
}