using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Application.Models.Requests
{
    public class CreateUsrInfoRequest
    {
        public string UsrId { get; set; } = null!;
        public string? DlrCode { get; set; }
        public string? CoCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? UsrName { get; set; }
        public int? UsrType { get; set; }
        public string? UsrNicno { get; set; }
        public string? UsrPassNo { get; set; }
        public string? UsrGender { get; set; }
        public DateTime? UsrDob { get; set; }
        public string? UsrRace { get; set; }
        public string? UsrEmail { get; set; }
        public string? UsrAddr { get; set; }
        public string? UsrPhone { get; set; }
        public string? UsrMobile { get; set; }
        public string? UsrFax { get; set; }
        public string? UsrStatus { get; set; }
        public string? UsrQualify { get; set; }
        public DateTime? UsrRegisterDate { get; set; }
        public DateTime? UsrTdrdate { get; set; }
        public DateTime? UsrResignDate { get; set; }
        public string? ClntCode { get; set; }
        public string? UsrLicenseNo { get; set; }
        public DateTime? UsrExpiryDate { get; set; }
        public string RmsType { get; set; } = null!;
        public int? UsrSuperiorId { get; set; }
        public int? UsrNotifierId { get; set; }
        public string? Category { get; set; }
        public string? UsrChannel { get; set; }
        public string? Pid { get; set; }
        public string? PidRms { get; set; }
    }
}
