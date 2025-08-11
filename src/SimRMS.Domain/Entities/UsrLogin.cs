/// <summary>
/// <para>
/// ===================================================================
/// Title:       UsrLogin
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents the user login details in the system.
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 

namespace SimRMS.Domain.Entities
{
    public class UsrLogin
    {
        public string UsrId { get; set; } = null!;

        public string? UsrPwd { get; set; }

        public string? UsrPwd1 { get; set; }

        public int? UsrPwdUnscsAtmpt { get; set; }

        public DateTime? UsrPwdLastChgDate { get; set; }

        public DateTime? UsrDisableWrngDate { get; set; }

        public string? UsrTrdgPin { get; set; }

        public int? UsrTrdgPinUnscsAtmpt { get; set; }

        /// <summary>
        /// Y - Created; N - Not Created; R - Reseted; 
        /// </summary>
        public string? UsrTrdgPinStat { get; set; }

        public DateTime? UsrTrdgPinLastChgDate { get; set; }

        public DateTime? UsrTrdgPinDisableWrngDate { get; set; }

        public int? UsrLogin1 { get; set; }

        public DateTime? UsrActiveTime { get; set; }

        public DateTime? UsrLastUpdatedDate { get; set; }

        public bool? UsrPwdReset { get; set; }

        public string? UsrActvnCode { get; set; }

        public string? UsrSecretAns1 { get; set; }

        public string? UsrSecretAns2 { get; set; }

        public string? UsrSecretAns3 { get; set; }

        public int? UsrForceLogout { get; set; }

        public string? UsrActvCode { get; set; }

        public DateTime? UsrLastLoginDate { get; set; }

        public bool UsrTrdgPinReset { get; set; }

        public int? UsrOtpresendAtt { get; set; }

        public int? UsrOtpvldtAtt { get; set; }

        public string? UsrOtpcode { get; set; }

        public DateTime? UsrOtpexpiration { get; set; }

        public int? UsrTwoFactorAuth { get; set; }

        public DateTime? UsrTwoFactorAuthBypassExpiryDate { get; set; }
    }
}
