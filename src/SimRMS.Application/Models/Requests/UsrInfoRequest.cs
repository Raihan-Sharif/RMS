using System;

namespace SimRMS.Application.Models.Requests
{
    /// <summary>
    /// Single request model for both Create and Update operations
    /// Use validation attributes and business logic to differentiate between create/update scenarios
    /// </summary>
    public class UsrInfoRequest
    {
        /// <summary>
        /// User ID - Required for Create, ignored for Update (comes from route)
        /// </summary>
        public string? UsrId { get; set; }

        /// <summary>
        /// Dealer Code
        /// </summary>
        public string? DlrCode { get; set; }

        /// <summary>
        /// Company Code
        /// </summary>
        public string? CoCode { get; set; }

        /// <summary>
        /// Company Branch Code
        /// </summary>
        public string? CoBrchCode { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        public string? UsrName { get; set; }

        /// <summary>
        /// User Type
        /// </summary>
        public int? UsrType { get; set; }

        /// <summary>
        /// User NIC Number
        /// </summary>
        public string? UsrNicno { get; set; }

        /// <summary>
        /// User Passport Number
        /// </summary>
        public string? UsrPassNo { get; set; }

        /// <summary>
        /// User Gender (M/F)
        /// </summary>
        public string? UsrGender { get; set; }

        /// <summary>
        /// User Date of Birth
        /// </summary>
        public DateTime? UsrDob { get; set; }

        /// <summary>
        /// User Race
        /// </summary>
        public string? UsrRace { get; set; }

        /// <summary>
        /// User Email
        /// </summary>
        public string? UsrEmail { get; set; }

        /// <summary>
        /// User Address
        /// </summary>
        public string? UsrAddr { get; set; }

        /// <summary>
        /// User Phone
        /// </summary>
        public string? UsrPhone { get; set; }

        /// <summary>
        /// User Mobile
        /// </summary>
        public string? UsrMobile { get; set; }

        /// <summary>
        /// User Fax
        /// </summary>
        public string? UsrFax { get; set; }

        /// <summary>
        /// User Status (A=Active, S=Suspend, C=Close)
        /// </summary>
        public string? UsrStatus { get; set; }

        /// <summary>
        /// User Qualification
        /// </summary>
        public string? UsrQualify { get; set; }

        /// <summary>
        /// User Registration Date
        /// </summary>
        public DateTime? UsrRegisterDate { get; set; }

        /// <summary>
        /// User TDR Date
        /// </summary>
        public DateTime? UsrTdrdate { get; set; }

        /// <summary>
        /// User Resignation Date
        /// </summary>
        public DateTime? UsrResignDate { get; set; }

        /// <summary>
        /// Client Code
        /// </summary>
        public string? ClntCode { get; set; }

        /// <summary>
        /// User License Number
        /// </summary>
        public string? UsrLicenseNo { get; set; }

        /// <summary>
        /// User License Expiry Date
        /// </summary>
        public DateTime? UsrExpiryDate { get; set; }

        /// <summary>
        /// RMS Type - Required for Create
        /// </summary>
        public string? RmsType { get; set; }

        /// <summary>
        /// User Superior ID
        /// </summary>
        public int? UsrSuperiorId { get; set; }

        /// <summary>
        /// User Notifier ID
        /// </summary>
        public int? UsrNotifierId { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// User Channel
        /// </summary>
        public string? UsrChannel { get; set; }

        /// <summary>
        /// PID
        /// </summary>
        public string? Pid { get; set; }

        /// <summary>
        /// PID RMS
        /// </summary>
        public string? PidRms { get; set; }
    }
}