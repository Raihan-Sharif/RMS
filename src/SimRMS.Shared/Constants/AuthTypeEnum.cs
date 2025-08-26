/// <summary>
/// <para>
/// ===================================================================
/// Title:       Auth Type Enumeration
/// Author:      Md. Raihan Sharif
/// Purpose:     Defines Authentication types for DML operations
/// Creation:    26/Aug/2025
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

public enum AuthTypeEnum : byte
{
    /// <summary>
    /// UnAuthorize
    /// </summary>
    UnAuthorize = 0,

    /// <summary>
    /// Approve
    /// </summary>
    Approve = 1,

    /// <summary>
    /// Deny
    /// </summary>
    Deny = 2,

}

