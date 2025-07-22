using Microsoft.Extensions.Logging;
using System.Data;
using LB.DAL.Core.Common;
using SimRMS.Domain.Interfaces;
using SimRMS.Domain.Entities;
using SimRMS.Shared.Models;

namespace SimRMS.Infrastructure.Repositories
{

    /// <summary>
    /// Specialized repository for UsrInfo entity with domain-specific operations
    /// </summary>
    public class UsrInfoRepository : LBRepository<UsrInfo>
    {
        public UsrInfoRepository(ILB_DAL dal, IEntityMapper entityMapper, ILogger<LBRepository<UsrInfo>> logger)
            : base(dal, entityMapper, logger)
        {
        }

        /// <summary>
        /// Get users with pagination and filtering - using stored procedure
        /// </summary>
        public async Task<PagedResult<UsrInfo>> GetUsersPagedAsync(int pageNumber, int pageSize, string? usrStatus = null, string? coCode = null, string? dlrCode = null, string? rmsType = null, string? searchTerm = null)
        {
            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("PageNumber", pageNumber),
                new LB_DALParam("PageSize", pageSize),
                new LB_DALParam("UsrStatus", usrStatus ?? (object)DBNull.Value),
                new LB_DALParam("CoCode", coCode ?? (object)DBNull.Value),
                new LB_DALParam("DlrCode", dlrCode ?? (object)DBNull.Value),
                new LB_DALParam("RmsType", rmsType ?? (object)DBNull.Value),
                new LB_DALParam("SearchTerm", searchTerm ?? (object)DBNull.Value)
            };

            return await GetPagedAsync(pageNumber, pageSize, "sp_GetUsrInfoPaged", parameters, CommandType.StoredProcedure);
        }

        /// <summary>
        /// Get user by ID - using inline query for better performance
        /// </summary>
        public async Task<UsrInfo?> GetByUserIdAsync(string usrId)
        {
            var query = @"
                SELECT UsrID, DlrCode, CoCode, CoBrchCode, UsrName, UsrType, UsrNICNo, UsrPassNo, 
                       UsrGender, UsrDOB, UsrRace, UsrEmail, UsrAddr, UsrPhone, UsrMobile, UsrFax,
                       UsrLastUpdatedDate, UsrCreationDate, UsrStatus, UsrQualify, UsrRegisterDate,
                       UsrTDRDate, UsrResignDate, ClntCode, UsrLicenseNo, UsrExpiryDate, RmsType,
                       UsrSuperiorID, UsrNotifierID, UsrAssct, UsrAssctPwd, UsrAccessFA, UsrBTXMode,
                       WithoutClntList, BFESName, ThirdPartyUsrID, GTCExpiryPeriod, UsrGTDMode,
                       MarketDepth, MktDepthStartDate, MktDepthEndDate, CrOrderDealer, Category,
                       UsrChannel, PID, PID_rms, PIDFlag, PIDFlag_rms, ChannelUpdFlag,
                       MimosMigrateDt, MimosMigrateDt_rms, OriUsrEmail
                FROM UsrInfo 
                WHERE UsrID = @UsrID";

            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrId)
            };

            return await FirstOrDefaultAsync(query, parameters, CommandType.Text);
        }

        /// <summary>
        /// Create new user - using stored procedure
        /// </summary>
        public async Task<UsrInfo> CreateUserAsync(UsrInfo usrInfo)
        {
            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrInfo.UsrId),
                new LB_DALParam("DlrCode", usrInfo.DlrCode ?? (object)DBNull.Value),
                new LB_DALParam("CoCode", usrInfo.CoCode ?? (object)DBNull.Value),
                new LB_DALParam("CoBrchCode", usrInfo.CoBrchCode ?? (object)DBNull.Value),
                new LB_DALParam("UsrName", usrInfo.UsrName ?? (object)DBNull.Value),
                new LB_DALParam("UsrType", usrInfo.UsrType ?? (object)DBNull.Value),
                new LB_DALParam("UsrNICNo", usrInfo.UsrNicno ?? (object)DBNull.Value),
                new LB_DALParam("UsrPassNo", usrInfo.UsrPassNo ?? (object)DBNull.Value),
                new LB_DALParam("UsrGender", usrInfo.UsrGender ?? (object)DBNull.Value),
                new LB_DALParam("UsrDOB", usrInfo.UsrDob ?? (object)DBNull.Value),
                new LB_DALParam("UsrRace", usrInfo.UsrRace ?? (object)DBNull.Value),
                new LB_DALParam("UsrEmail", usrInfo.UsrEmail ?? (object)DBNull.Value),
                new LB_DALParam("UsrAddr", usrInfo.UsrAddr ?? (object)DBNull.Value),
                new LB_DALParam("UsrPhone", usrInfo.UsrPhone ?? (object)DBNull.Value),
                new LB_DALParam("UsrMobile", usrInfo.UsrMobile ?? (object)DBNull.Value),
                new LB_DALParam("UsrFax", usrInfo.UsrFax ?? (object)DBNull.Value),
                new LB_DALParam("UsrStatus", usrInfo.UsrStatus ?? "A"),
                new LB_DALParam("UsrQualify", usrInfo.UsrQualify ?? (object)DBNull.Value),
                new LB_DALParam("UsrRegisterDate", usrInfo.UsrRegisterDate ?? (object)DBNull.Value),
                new LB_DALParam("UsrTDRDate", usrInfo.UsrTdrdate ?? (object)DBNull.Value),
                new LB_DALParam("UsrResignDate", usrInfo.UsrResignDate ?? (object)DBNull.Value),
                new LB_DALParam("ClntCode", usrInfo.ClntCode ?? (object)DBNull.Value),
                new LB_DALParam("UsrLicenseNo", usrInfo.UsrLicenseNo ?? (object)DBNull.Value),
                new LB_DALParam("UsrExpiryDate", usrInfo.UsrExpiryDate ?? (object)DBNull.Value),
                new LB_DALParam("RmsType", usrInfo.RmsType),
                new LB_DALParam("UsrSuperiorID", usrInfo.UsrSuperiorId ?? (object)DBNull.Value),
                new LB_DALParam("UsrNotifierID", usrInfo.UsrNotifierId ?? (object)DBNull.Value),
                new LB_DALParam("Category", usrInfo.Category ?? (object)DBNull.Value),
                new LB_DALParam("UsrChannel", usrInfo.UsrChannel ?? (object)DBNull.Value),
                new LB_DALParam("PID", usrInfo.Pid ?? (object)DBNull.Value),
                new LB_DALParam("PID_rms", usrInfo.PidRms ?? (object)DBNull.Value),
                new LB_DALParam("UsrCreationDate", DateTime.UtcNow),
                new LB_DALParam("UsrLastUpdatedDate", DateTime.UtcNow)
            };

            await ExecuteNonQueryAsync("sp_CreateUsrInfo", parameters, CommandType.StoredProcedure);

            // Set the timestamps
            usrInfo.UsrCreationDate = DateTime.UtcNow;
            usrInfo.UsrLastUpdatedDate = DateTime.UtcNow;

            return usrInfo;
        }

        /// <summary>
        /// Update user - using stored procedure
        /// </summary>
        public async Task<UsrInfo> UpdateUserAsync(UsrInfo usrInfo)
        {
            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrInfo.UsrId),
                new LB_DALParam("DlrCode", usrInfo.DlrCode ?? (object)DBNull.Value),
                new LB_DALParam("CoCode", usrInfo.CoCode ?? (object)DBNull.Value),
                new LB_DALParam("CoBrchCode", usrInfo.CoBrchCode ?? (object)DBNull.Value),
                new LB_DALParam("UsrName", usrInfo.UsrName ?? (object)DBNull.Value),
                new LB_DALParam("UsrType", usrInfo.UsrType ?? (object)DBNull.Value),
                new LB_DALParam("UsrNICNo", usrInfo.UsrNicno ?? (object)DBNull.Value),
                new LB_DALParam("UsrPassNo", usrInfo.UsrPassNo ?? (object)DBNull.Value),
                new LB_DALParam("UsrGender", usrInfo.UsrGender ?? (object)DBNull.Value),
                new LB_DALParam("UsrDOB", usrInfo.UsrDob ?? (object)DBNull.Value),
                new LB_DALParam("UsrRace", usrInfo.UsrRace ?? (object)DBNull.Value),
                new LB_DALParam("UsrEmail", usrInfo.UsrEmail ?? (object)DBNull.Value),
                new LB_DALParam("UsrAddr", usrInfo.UsrAddr ?? (object)DBNull.Value),
                new LB_DALParam("UsrPhone", usrInfo.UsrPhone ?? (object)DBNull.Value),
                new LB_DALParam("UsrMobile", usrInfo.UsrMobile ?? (object)DBNull.Value),
                new LB_DALParam("UsrFax", usrInfo.UsrFax ?? (object)DBNull.Value),
                new LB_DALParam("UsrStatus", usrInfo.UsrStatus ?? (object)DBNull.Value),
                new LB_DALParam("UsrQualify", usrInfo.UsrQualify ?? (object)DBNull.Value),
                new LB_DALParam("UsrRegisterDate", usrInfo.UsrRegisterDate ?? (object)DBNull.Value),
                new LB_DALParam("UsrTDRDate", usrInfo.UsrTdrdate ?? (object)DBNull.Value),
                new LB_DALParam("UsrResignDate", usrInfo.UsrResignDate ?? (object)DBNull.Value),
                new LB_DALParam("ClntCode", usrInfo.ClntCode ?? (object)DBNull.Value),
                new LB_DALParam("UsrLicenseNo", usrInfo.UsrLicenseNo ?? (object)DBNull.Value),
                new LB_DALParam("UsrExpiryDate", usrInfo.UsrExpiryDate ?? (object)DBNull.Value),
                new LB_DALParam("RmsType", usrInfo.RmsType),
                new LB_DALParam("UsrSuperiorID", usrInfo.UsrSuperiorId ?? (object)DBNull.Value),
                new LB_DALParam("UsrNotifierID", usrInfo.UsrNotifierId ?? (object)DBNull.Value),
                new LB_DALParam("Category", usrInfo.Category ?? (object)DBNull.Value),
                new LB_DALParam("UsrChannel", usrInfo.UsrChannel ?? (object)DBNull.Value),
                new LB_DALParam("PID", usrInfo.Pid ?? (object)DBNull.Value),
                new LB_DALParam("PID_rms", usrInfo.PidRms ?? (object)DBNull.Value),
                new LB_DALParam("UsrLastUpdatedDate", DateTime.UtcNow)
            };

            await ExecuteNonQueryAsync("sp_UpdateUsrInfo", parameters, CommandType.StoredProcedure);

            // Update the timestamp
            usrInfo.UsrLastUpdatedDate = DateTime.UtcNow;

            return usrInfo;
        }

        /// <summary>
        /// Delete user - using stored procedure
        /// </summary>
        public async Task<bool> DeleteUserAsync(string usrId)
        {
            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrId)
            };

            var rowsAffected = await ExecuteNonQueryAsync("sp_DeleteUsrInfo", parameters, CommandType.StoredProcedure);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Check if user exists by email - using inline query
        /// </summary>
        public async Task<bool> ExistsByEmailAsync(string email, string? excludeUsrId = null)
        {
            var query = @"
                SELECT COUNT(1) 
                FROM UsrInfo 
                WHERE UsrEmail = @Email 
                AND (@ExcludeUsrId IS NULL OR UsrID != @ExcludeUsrId)";

            var parameters = new List<LB_DALParam>
            {
                new LB_DALParam("Email", email),
                new LB_DALParam("ExcludeUsrId", excludeUsrId ?? (object)DBNull.Value)
            };

            var count = await ExecuteScalarAsync<int>(query, parameters, CommandType.Text);
            return count > 0;
        }

        /// <summary>
        /// Get user statistics - using inline query
        /// </summary>
        public async Task<Dictionary<string, object>> GetUserStatisticsAsync()
        {
            var query = @"
                SELECT 
                    COUNT(*) as TotalUsers,
                    SUM(CASE WHEN UsrStatus = 'A' THEN 1 ELSE 0 END) as ActiveUsers,
                    SUM(CASE WHEN UsrStatus = 'S' THEN 1 ELSE 0 END) as SuspendedUsers,
                    SUM(CASE WHEN UsrStatus = 'C' THEN 1 ELSE 0 END) as ClosedUsers
                FROM UsrInfo";

            using var reader = await GetDataReaderAsync(query, null, CommandType.Text);

            if (await reader.ReadAsync())
            {
                return _entityMapper.MapToDictionary(reader);
            }

            return new Dictionary<string, object>();
        }

        protected override string GenerateInsertQuery()
        {
            return @"
                INSERT INTO UsrInfo (
                    UsrID, DlrCode, CoCode, CoBrchCode, UsrName, UsrType, UsrNICNo, UsrPassNo,
                    UsrGender, UsrDOB, UsrRace, UsrEmail, UsrAddr, UsrPhone, UsrMobile, UsrFax,
                    UsrStatus, UsrQualify, UsrRegisterDate, UsrTDRDate, UsrResignDate, ClntCode,
                    UsrLicenseNo, UsrExpiryDate, RmsType, UsrSuperiorID, UsrNotifierID, Category,
                    UsrChannel, PID, PID_rms, UsrCreationDate, UsrLastUpdatedDate
                ) VALUES (
                    @UsrID, @DlrCode, @CoCode, @CoBrchCode, @UsrName, @UsrType, @UsrNICNo, @UsrPassNo,
                    @UsrGender, @UsrDOB, @UsrRace, @UsrEmail, @UsrAddr, @UsrPhone, @UsrMobile, @UsrFax,
                    @UsrStatus, @UsrQualify, @UsrRegisterDate, @UsrTDRDate, @UsrResignDate, @ClntCode,
                    @UsrLicenseNo, @UsrExpiryDate, @RmsType, @UsrSuperiorID, @UsrNotifierID, @Category,
                    @UsrChannel, @PID, @PID_rms, @UsrCreationDate, @UsrLastUpdatedDate
                )";
        }

        protected override string GenerateUpdateQuery()
        {
            return @"
                UPDATE UsrInfo SET 
                    DlrCode = @DlrCode, CoCode = @CoCode, CoBrchCode = @CoBrchCode, 
                    UsrName = @UsrName, UsrType = @UsrType, UsrNICNo = @UsrNICNo, 
                    UsrPassNo = @UsrPassNo, UsrGender = @UsrGender, UsrDOB = @UsrDOB,
                    UsrRace = @UsrRace, UsrEmail = @UsrEmail, UsrAddr = @UsrAddr,
                    UsrPhone = @UsrPhone, UsrMobile = @UsrMobile, UsrFax = @UsrFax,
                    UsrStatus = @UsrStatus, UsrQualify = @UsrQualify, 
                    UsrRegisterDate = @UsrRegisterDate, UsrTDRDate = @UsrTDRDate,
                    UsrResignDate = @UsrResignDate, ClntCode = @ClntCode,
                    UsrLicenseNo = @UsrLicenseNo, UsrExpiryDate = @UsrExpiryDate,
                    RmsType = @RmsType, UsrSuperiorID = @UsrSuperiorID, 
                    UsrNotifierID = @UsrNotifierID, Category = @Category,
                    UsrChannel = @UsrChannel, PID = @PID, PID_rms = @PID_rms,
                    UsrLastUpdatedDate = @UsrLastUpdatedDate
                WHERE UsrID = @UsrID";
        }
    }
}
