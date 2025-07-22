using Microsoft.Extensions.Logging;
using SimRMS.Domain.Entities;
using SimRMS.Domain.Interfaces;
using System.Data;
using LB.DAL.Core.Common;
using SimRMS.Infrastructure.Data;

namespace SimRMS.Infrastructure.Repositories
{
    /// <summary>
    /// Infrastructure implementation of IUsrLoginRepository
    /// Contains authentication-specific operations using LB.DAL
    /// </summary>
    public class UsrLoginRepository : LBRepositoryBase<UsrLogin>, IUsrLoginRepository
    {
        public UsrLoginRepository(ILB_DAL dal, LBEntityMapper entityMapper, ILogger<LBRepositoryBase<UsrLogin>> logger)
            : base(dal, entityMapper, logger)
        {
        }

        #region IUsrLoginRepository Implementation

        public async Task<UsrLogin?> GetByUserIdAsync(string usrId, CancellationToken cancellationToken = default)
        {
            var query = @"
                SELECT UsrID, UsrPwd, UsrPwd1, UsrPwdUnscsAtmpt, UsrPwdLastChgDate, 
                       UsrDisableWrngDate, UsrTrdgPin, UsrTrdgPinUnscsAtmpt, UsrTrdgPinStat,
                       UsrTrdgPinLastChgDate, UsrTrdgPinDisableWrngDate, UsrLogin, 
                       UsrActiveTime, UsrLastUpdatedDate, UsrPwdReset, UsrActvnCode,
                       UsrSecretAns1, UsrSecretAns2, UsrSecretAns3, UsrForceLogout,
                       UsrActvCode, UsrLastLoginDate, UsrTrdgPinReset, UsrOTPResendAtt,
                       UsrOTPVldtAtt, UsrOTPCode, UsrOTPExpiration, UsrTwoFactorAuth,
                       UsrTwoFactorAuthBypassExpiryDate
                FROM UsrLogin 
                WHERE UsrID = @UsrID";

            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrId)
            };

            try
            {
                using var reader = await _dal.LB_GetDbDataReaderAsync(query, parameters, CommandType.Text);

                if (await reader.ReadAsync())
                {
                    return _entityMapper.MapToEntity<UsrLogin>(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UsrLogin by UsrId: {UsrId}", usrId);
                throw;
            }
        }

        public async Task<(bool IsValid, UsrLogin? UserLogin)> ValidateCredentialsAsync(string usrId, string password, CancellationToken cancellationToken = default)
        {
            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrId),
                new LB_DALParam("Password", password),
                new LB_DALParam("IsValid", false, ParameterDirection.Output),
                new LB_DALParam("LoginAttempts", 0, ParameterDirection.Output)
            };

            try
            {
                await _dal.LB_ExecuteNonQueryAsync("sp_ValidateUserCredentials", parameters, CommandType.StoredProcedure);

                // Get output parameters
                var isValid = parameters.FirstOrDefault(p => p.ParameterName == "IsValid")?.Value as bool? ?? false;
                var loginAttempts = parameters.FirstOrDefault(p => p.ParameterName == "LoginAttempts")?.Value as int? ?? 0;

                if (isValid)
                {
                    var userLogin = await GetByUserIdAsync(usrId, cancellationToken);
                    return (true, userLogin);
                }

                return (false, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating credentials for user: {UsrId}", usrId);
                return (false, null);
            }
        }

        public async Task<bool> UpdateLastLoginAsync(string usrId, CancellationToken cancellationToken = default)
        {
            var query = @"
                UPDATE UsrLogin 
                SET UsrLastLoginDate = @LoginDate,
                    UsrActiveTime = @ActiveTime,
                    UsrLastUpdatedDate = @UpdateDate
                WHERE UsrID = @UsrID";

            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrId),
                new LB_DALParam("LoginDate", DateTime.UtcNow),
                new LB_DALParam("ActiveTime", DateTime.UtcNow),
                new LB_DALParam("UpdateDate", DateTime.UtcNow)
            };

            try
            {
                var rowsAffected = await _dal.LB_ExecuteNonQueryAsync(query, parameters, CommandType.Text);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login for user: {UsrId}", usrId);
                throw;
            }
        }

        public async Task<bool> UpdateUserActivityAsync(string usrId, CancellationToken cancellationToken = default)
        {
            var query = @"
                UPDATE UsrLogin 
                SET UsrActiveTime = @ActiveTime,
                    UsrLastUpdatedDate = @UpdateDate
                WHERE UsrID = @UsrID";

            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrId),
                new LB_DALParam("ActiveTime", DateTime.UtcNow),
                new LB_DALParam("UpdateDate", DateTime.UtcNow)
            };

            try
            {
                var rowsAffected = await _dal.LB_ExecuteNonQueryAsync(query, parameters, CommandType.Text);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user activity for user: {UsrId}", usrId);
                throw;
            }
        }

        public async Task<bool> ResetPasswordAttemptsAsync(string usrId, CancellationToken cancellationToken = default)
        {
            var query = @"
                UPDATE UsrLogin 
                SET UsrPwdUnscsAtmpt = 0,
                    UsrDisableWrngDate = NULL,
                    UsrLastUpdatedDate = @UpdateDate
                WHERE UsrID = @UsrID";

            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrId),
                new LB_DALParam("UpdateDate", DateTime.UtcNow)
            };

            try
            {
                var rowsAffected = await _dal.LB_ExecuteNonQueryAsync(query, parameters, CommandType.Text);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password attempts for user: {UsrId}", usrId);
                throw;
            }
        }

        public async Task<int> IncrementFailedLoginAttemptsAsync(string usrId, CancellationToken cancellationToken = default)
        {
            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrId),
                new LB_DALParam("FailedAttempts", 0, ParameterDirection.Output)
            };

            try
            {
                await _dal.LB_ExecuteNonQueryAsync("sp_IncrementFailedLoginAttempts", parameters, CommandType.StoredProcedure);
                return parameters.FirstOrDefault(p => p.ParameterName == "FailedAttempts")?.Value as int? ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing failed login attempts for user: {UsrId}", usrId);
                throw;
            }
        }

        public async Task<UsrLogin> CreateUserLoginAsync(UsrLogin usrLogin, CancellationToken cancellationToken = default)
        {
            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrLogin.UsrId),
                new LB_DALParam("UsrPwd", usrLogin.UsrPwd ?? (object)DBNull.Value),
                new LB_DALParam("UsrPwd1", usrLogin.UsrPwd1 ?? (object)DBNull.Value),
                new LB_DALParam("UsrPwdUnscsAtmpt", usrLogin.UsrPwdUnscsAtmpt ?? 0),
                new LB_DALParam("UsrPwdLastChgDate", usrLogin.UsrPwdLastChgDate ?? (object)DBNull.Value),
                new LB_DALParam("UsrDisableWrngDate", usrLogin.UsrDisableWrngDate ?? (object)DBNull.Value),
                new LB_DALParam("UsrTrdgPin", usrLogin.UsrTrdgPin ?? (object)DBNull.Value),
                new LB_DALParam("UsrTrdgPinUnscsAtmpt", usrLogin.UsrTrdgPinUnscsAtmpt ?? 0),
                new LB_DALParam("UsrTrdgPinStat", usrLogin.UsrTrdgPinStat ?? "N"),
                new LB_DALParam("UsrTrdgPinLastChgDate", usrLogin.UsrTrdgPinLastChgDate ?? (object)DBNull.Value),
                new LB_DALParam("UsrTrdgPinDisableWrngDate", usrLogin.UsrTrdgPinDisableWrngDate ?? (object)DBNull.Value),
                new LB_DALParam("UsrLogin", usrLogin.UsrLogin1 ?? 0),
                new LB_DALParam("UsrActiveTime", usrLogin.UsrActiveTime ?? (object)DBNull.Value),
                new LB_DALParam("UsrPwdReset", usrLogin.UsrPwdReset ?? false),
                new LB_DALParam("UsrActvnCode", usrLogin.UsrActvnCode ?? (object)DBNull.Value),
                new LB_DALParam("UsrSecretAns1", usrLogin.UsrSecretAns1 ?? (object)DBNull.Value),
                new LB_DALParam("UsrSecretAns2", usrLogin.UsrSecretAns2 ?? (object)DBNull.Value),
                new LB_DALParam("UsrSecretAns3", usrLogin.UsrSecretAns3 ?? (object)DBNull.Value),
                new LB_DALParam("UsrForceLogout", usrLogin.UsrForceLogout ?? 0),
                new LB_DALParam("UsrActvCode", usrLogin.UsrActvCode ?? (object)DBNull.Value),
                new LB_DALParam("UsrLastLoginDate", usrLogin.UsrLastLoginDate ?? (object)DBNull.Value),
                new LB_DALParam("UsrTrdgPinReset", usrLogin.UsrTrdgPinReset),
                new LB_DALParam("UsrOTPResendAtt", usrLogin.UsrOtpresendAtt ?? 0),
                new LB_DALParam("UsrOTPVldtAtt", usrLogin.UsrOtpvldtAtt ?? 0),
                new LB_DALParam("UsrOTPCode", usrLogin.UsrOtpcode ?? (object)DBNull.Value),
                new LB_DALParam("UsrOTPExpiration", usrLogin.UsrOtpexpiration ?? (object)DBNull.Value),
                new LB_DALParam("UsrTwoFactorAuth", usrLogin.UsrTwoFactorAuth ?? 1),
                new LB_DALParam("UsrTwoFactorAuthBypassExpiryDate", usrLogin.UsrTwoFactorAuthBypassExpiryDate ?? (object)DBNull.Value),
                new LB_DALParam("UsrLastUpdatedDate", DateTime.UtcNow)
            };

            try
            {
                await _dal.LB_ExecuteNonQueryAsync("sp_CreateUsrLogin", parameters, CommandType.StoredProcedure);

                usrLogin.UsrLastUpdatedDate = DateTime.UtcNow;
                return usrLogin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating UsrLogin: {UsrId}", usrLogin.UsrId);
                throw;
            }
        }

        public async Task<IEnumerable<UsrLogin>> GetRecentActiveUsersAsync(int hours = 24, CancellationToken cancellationToken = default)
        {
            var query = @"
                SELECT ul.* 
                FROM UsrLogin ul
                INNER JOIN UsrInfo ui ON ul.UsrID = ui.UsrID
                WHERE ul.UsrActiveTime >= @SinceDate
                  AND ui.UsrStatus = 'A'
                ORDER BY ul.UsrActiveTime DESC";

            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("SinceDate", DateTime.UtcNow.AddHours(-hours))
            };

            try
            {
                using var reader = await _dal.LB_GetDbDataReaderAsync(query, parameters, CommandType.Text);
                return await _entityMapper.MapToListAsync<UsrLogin>(reader);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent active users");
                throw;
            }
        }

        #endregion

        #region Override Base Methods

        protected override string GenerateInsertQuery()
        {
            return @"
                INSERT INTO UsrLogin (
                    UsrID, UsrPwd, UsrPwd1, UsrPwdUnscsAtmpt, UsrPwdLastChgDate,
                    UsrDisableWrngDate, UsrTrdgPin, UsrTrdgPinUnscsAtmpt, UsrTrdgPinStat,
                    UsrTrdgPinLastChgDate, UsrTrdgPinDisableWrngDate, UsrLogin,
                    UsrActiveTime, UsrPwdReset, UsrActvnCode, UsrSecretAns1,
                    UsrSecretAns2, UsrSecretAns3, UsrForceLogout, UsrActvCode,
                    UsrLastLoginDate, UsrTrdgPinReset, UsrOTPResendAtt, UsrOTPVldtAtt,
                    UsrOTPCode, UsrOTPExpiration, UsrTwoFactorAuth, 
                    UsrTwoFactorAuthBypassExpiryDate, UsrLastUpdatedDate
                ) VALUES (
                    @UsrID, @UsrPwd, @UsrPwd1, @UsrPwdUnscsAtmpt, @UsrPwdLastChgDate,
                    @UsrDisableWrngDate, @UsrTrdgPin, @UsrTrdgPinUnscsAtmpt, @UsrTrdgPinStat,
                    @UsrTrdgPinLastChgDate, @UsrTrdgPinDisableWrngDate, @UsrLogin,
                    @UsrActiveTime, @UsrPwdReset, @UsrActvnCode, @UsrSecretAns1,
                    @UsrSecretAns2, @UsrSecretAns3, @UsrForceLogout, @UsrActvCode,
                    @UsrLastLoginDate, @UsrTrdgPinReset, @UsrOTPResendAtt, @UsrOTPVldtAtt,
                    @UsrOTPCode, @UsrOTPExpiration, @UsrTwoFactorAuth,
                    @UsrTwoFactorAuthBypassExpiryDate, @UsrLastUpdatedDate
                )";
        }

        protected override string GenerateUpdateQuery()
        {
            return @"
                UPDATE UsrLogin SET 
                    UsrPwd = @UsrPwd,
                    UsrPwd1 = @UsrPwd1,
                    UsrPwdUnscsAtmpt = @UsrPwdUnscsAtmpt,
                    UsrPwdLastChgDate = @UsrPwdLastChgDate,
                    UsrDisableWrngDate = @UsrDisableWrngDate,
                    UsrTrdgPin = @UsrTrdgPin,
                    UsrTrdgPinUnscsAtmpt = @UsrTrdgPinUnscsAtmpt,
                    UsrTrdgPinStat = @UsrTrdgPinStat,
                    UsrTrdgPinLastChgDate = @UsrTrdgPinLastChgDate,
                    UsrTrdgPinDisableWrngDate = @UsrTrdgPinDisableWrngDate,
                    UsrLogin = @UsrLogin,
                    UsrActiveTime = @UsrActiveTime,
                    UsrPwdReset = @UsrPwdReset,
                    UsrActvnCode = @UsrActvnCode,
                    UsrSecretAns1 = @UsrSecretAns1,
                    UsrSecretAns2 = @UsrSecretAns2,
                    UsrSecretAns3 = @UsrSecretAns3,
                    UsrForceLogout = @UsrForceLogout,
                    UsrActvCode = @UsrActvCode,
                    UsrLastLoginDate = @UsrLastLoginDate,
                    UsrTrdgPinReset = @UsrTrdgPinReset,
                    UsrOTPResendAtt = @UsrOTPResendAtt,
                    UsrOTPVldtAtt = @UsrOTPVldtAtt,
                    UsrOTPCode = @UsrOTPCode,
                    UsrOTPExpiration = @UsrOTPExpiration,
                    UsrTwoFactorAuth = @UsrTwoFactorAuth,
                    UsrTwoFactorAuthBypassExpiryDate = @UsrTwoFactorAuthBypassExpiryDate,
                    UsrLastUpdatedDate = @UsrLastUpdatedDate
                WHERE UsrID = @UsrID";
        }

        #endregion
    }
}
