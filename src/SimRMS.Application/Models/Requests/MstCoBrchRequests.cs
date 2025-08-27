
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Market Stock Company Branch Request Models
/// Author:      Md. Raihan Sharif
/// Purpose:     Request models for Market Stock Company Branch operations
/// Creation:    13/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
namespace SimRMS.Application.Models.Requests
{
    public class GetMstCoBrchByIdRequest
    {
        public string CoCode { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
    }

    public class GetMstCoBrchListRequest
    {
        public string? SearchTerm { get; set; }
        public string? CoCode { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CreateMstCoBrchRequest
    {
        //public string CoCode { get; set; } = null!;
        //public string CoBrchCode { get; set; } = null!;
        public string CoBrchDesc { get; set; } = null!;
        public string? CoBrchAddr { get; set; }
        public string? CoBrchPhone { get; set; }
        public string? Remarks { get; set; }
    }

    public class UpdateMstCoBrchRequest
    {
        public string CoCode { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
        public string? CoBrchDesc { get; set; }
        public string? CoBrchAddr { get; set; }
        public string? CoBrchPhone { get; set; }
        public string? Remarks { get; set; }
    }

    public class DeleteMstCoBrchRequest
    {
        public string CoCode { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
        public string? Remarks { get; set; }
    }

    public class AuthorizeMstCoBrchRequest
    {
        public string CoCode { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? IPAddress { get; set; }
    }

    public class GetBranchWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? CoCode { get; set; }
        public int IsAuth { get; set; } = (byte)AuthTypeEnum.UnAuthorize;
    }
}