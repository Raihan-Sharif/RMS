using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRMS.Application.Models.DTOs
{
    public class UsrInfoDto
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
        public DateTime? UsrLastUpdatedDate { get; set; }
        public DateTime? UsrCreationDate { get; set; }
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
        public int? UsrAssct { get; set; }
        public string? UsrAssctPwd { get; set; }
        public string? UsrAccessFa { get; set; }
        public string? UsrBtxmode { get; set; }
        public string? WithoutClntList { get; set; }
        public string? Bfesname { get; set; }
        public string? ThirdPartyUsrId { get; set; }
        public int? GtcexpiryPeriod { get; set; }
        public string? UsrGtdmode { get; set; }
        public int? MarketDepth { get; set; }
        public DateTime? MktDepthStartDate { get; set; }
        public DateTime? MktDepthEndDate { get; set; }
        public int? CrOrderDealer { get; set; }
        public string? Category { get; set; }
        public string? UsrChannel { get; set; }
        public string? Pid { get; set; }
        public string? PidRms { get; set; }
        public string? Pidflag { get; set; }
        public string? PidflagRms { get; set; }
        public string? ChannelUpdFlag { get; set; }
        public DateTime? MimosMigrateDt { get; set; }
        public DateTime? MimosMigrateDtRms { get; set; }
        public string? OriUsrEmail { get; set; }
    }
}
