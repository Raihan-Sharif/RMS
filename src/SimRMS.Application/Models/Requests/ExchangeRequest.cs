using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Exchange Request Models
/// Author:      Raihan
/// Purpose:     Request models for Exchange CRUD operations
/// Creation:    01/Dec/2025
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
    /// <summary>
    /// Request model for creating a new exchange
    /// </summary>
    public class CreateExchangeRequest
    {
        public string XchgCode { get; set; } = string.Empty;
        public int XchgPrefix { get; set; }
        public string BrokerCode { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Request model for updating an existing exchange
    /// </summary>
    public class UpdateExchangeRequest
    {
        public string XchgCode { get; set; } = string.Empty;
        public int XchgPrefix { get; set; }
        public string BrokerCode { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Request model for deleting an exchange
    /// </summary>
    public class DeleteExchangeRequest
    {
        public string XchgCode { get; set; } = null!;
        public int XchgPrefix { get; set; }
        public string BrokerCode { get; set; } = null!;
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Request model for getting exchange workflow list with validation
    /// </summary>
    public class GetExchangeWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchText { get; set; }
        public string? SearchColumn { get; set; }
        public string? XchgCode { get; set; }
        public string? SortDirection { get; set; }
        public int IsAuth { get; set; } = 0; // 0 = UnAuthorize, 2 = Deny
    }

    /// <summary>
    /// Request model for authorizing Exchange
    /// Maps to LB_SP_AuthExchange stored procedure parameters
    /// </summary>
    public class AuthorizeExchangeRequest
    {
        public string XchgCode { get; set; } = null!;
        public int XchgPrefix { get; set; }
        public string BrokerCode { get; set; } = null!;
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }

}
