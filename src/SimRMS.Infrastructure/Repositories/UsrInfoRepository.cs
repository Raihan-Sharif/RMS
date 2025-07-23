using LB.DAL.Core.Common;
using SimRMS.Domain.Entities;
using SimRMS.Shared.Models;
using System.Data.Common;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimRMS.Domain.Interfaces.Repo;

namespace SimRMS.Infrastructure.Repositories
{
    /// <summary>
    /// Infrastructure implementation of IUsrInfoRepository
    /// Follows your existing CityRepository pattern with Clean Architecture
    /// </summary>
    public class UsrInfoRepository : IUsrInfoRepository
    {
        private readonly ILB_DAL _dal;
        private readonly ILogger<UsrInfoRepository> _logger;

        public UsrInfoRepository([FromKeyedServices("DBApplication")] ILB_DAL dal, ILogger<UsrInfoRepository> logger)
        {
            _dal = dal;
            _logger = logger;
            _dal.LB_GetConnectionAsync().Wait();
        }

        public async Task<UsrInfo?> GetByUserIdAsync(string usrId, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Getting UsrInfo by UsrId: {UsrId}", usrId);

            string sSql = @"SELECT UsrID, DlrCode, CoCode, CoBrchCode, UsrName, UsrType, UsrNICNo, UsrPassNo, 
                           UsrGender, UsrDOB, UsrRace, UsrEmail, UsrAddr, UsrPhone, UsrMobile, UsrFax,
                           UsrLastUpdatedDate, UsrCreationDate, UsrStatus, UsrQualify, UsrRegisterDate,
                           UsrTDRDate, UsrResignDate, ClntCode, UsrLicenseNo, UsrExpiryDate, RmsType,
                           UsrSuperiorID, UsrNotifierID 
                           FROM UsrInfo WHERE UsrID = @UsrID";

            List<LB_DALParam> lstParam = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrId)
            };

            using DbDataReader dr = await _dal.LB_GetDbDataReaderAsync(sSql, lstParam, CommandType.Text);
            if (dr.Read())
            {
                return MapUsrInfoFromReader(dr);
            }
            return null;
        }

        public async Task<PagedResult<UsrInfo>> GetUsersPagedAsync(int pageNumber, int pageSize,
            string? usrStatus = null, string? coCode = null, string? dlrCode = null,
            string? rmsType = null, string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Getting paged UsrInfo - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

            List<LB_DALParam> lstParam = new List<LB_DALParam>();
            List<string> whereConditions = new List<string>();

            // Build WHERE conditions dynamically
            if (!string.IsNullOrEmpty(usrStatus))
            {
                whereConditions.Add("UsrStatus = @UsrStatus");
                lstParam.Add(new LB_DALParam("UsrStatus", usrStatus));
            }
            if (!string.IsNullOrEmpty(coCode))
            {
                whereConditions.Add("CoCode = @CoCode");
                lstParam.Add(new LB_DALParam("CoCode", coCode));
            }
            if (!string.IsNullOrEmpty(dlrCode))
            {
                whereConditions.Add("DlrCode = @DlrCode");
                lstParam.Add(new LB_DALParam("DlrCode", dlrCode));
            }
            if (!string.IsNullOrEmpty(rmsType))
            {
                whereConditions.Add("RmsType = @RmsType");
                lstParam.Add(new LB_DALParam("RmsType", rmsType));
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereConditions.Add("(UsrID LIKE @SearchTerm OR UsrName LIKE @SearchTerm OR UsrEmail LIKE @SearchTerm)");
                lstParam.Add(new LB_DALParam("SearchTerm", $"%{searchTerm}%"));
            }

            string whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";

            // Get total count
            string countSql = $"SELECT COUNT(*) FROM UsrInfo {whereClause}";
            var totalCount = Convert.ToInt32(await _dal.LB_ExecuteScalarAsync(countSql, lstParam, CommandType.Text));

            // Get paged data
            int offset = (pageNumber - 1) * pageSize;
            lstParam.Add(new LB_DALParam("Offset", offset));
            lstParam.Add(new LB_DALParam("PageSize", pageSize));

            string pagedSql = $@"SELECT UsrID, DlrCode, CoCode, CoBrchCode, UsrName, UsrType, UsrNICNo, UsrPassNo, 
                               UsrGender, UsrDOB, UsrRace, UsrEmail, UsrAddr, UsrPhone, UsrMobile, UsrFax,
                               UsrLastUpdatedDate, UsrCreationDate, UsrStatus, UsrQualify, UsrRegisterDate,
                               UsrTDRDate, UsrResignDate, ClntCode, UsrLicenseNo, UsrExpiryDate, RmsType,
                               UsrSuperiorID, UsrNotifierID 
                               FROM UsrInfo {whereClause}
                               ORDER BY UsrID
                               OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            List<UsrInfo> users = new List<UsrInfo>();
            using DbDataReader dr = await _dal.LB_GetDbDataReaderAsync(pagedSql, lstParam, CommandType.Text);
            while (dr.Read())
            {
                users.Add(MapUsrInfoFromReader(dr));
            }

            return new PagedResult<UsrInfo>
            {
                Data = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<UsrInfo> CreateUserAsync(UsrInfo usrInfo, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Creating UsrInfo: {UsrId}", usrInfo.UsrId);

            string sSql = @"INSERT INTO UsrInfo (UsrID, DlrCode, CoCode, CoBrchCode, UsrName, UsrType, UsrNICNo, 
                           UsrPassNo, UsrGender, UsrDOB, UsrRace, UsrEmail, UsrAddr, UsrPhone, UsrMobile, UsrFax,
                           UsrStatus, UsrQualify, UsrRegisterDate, UsrTDRDate, UsrResignDate, ClntCode,
                           UsrLicenseNo, UsrExpiryDate, RmsType, UsrSuperiorID, UsrNotifierID, 
                           UsrCreationDate, UsrLastUpdatedDate)
                           VALUES (@UsrID, @DlrCode, @CoCode, @CoBrchCode, @UsrName, @UsrType, @UsrNICNo,
                           @UsrPassNo, @UsrGender, @UsrDOB, @UsrRace, @UsrEmail, @UsrAddr, @UsrPhone, @UsrMobile, @UsrFax,
                           @UsrStatus, @UsrQualify, @UsrRegisterDate, @UsrTDRDate, @UsrResignDate, @ClntCode,
                           @UsrLicenseNo, @UsrExpiryDate, @RmsType, @UsrSuperiorID, @UsrNotifierID,
                           @UsrCreationDate, @UsrLastUpdatedDate)";

            List<LB_DALParam> lstParam = CreateUsrInfoParameters(usrInfo, true);
            int rowsAffected = await _dal.LB_ExecuteNonQueryAsync(sSql, lstParam, CommandType.Text);

            _logger.LogDebug("Created UsrInfo: {UsrId}, Rows affected: {RowsAffected}", usrInfo.UsrId, rowsAffected);
            return usrInfo;
        }

        public async Task<UsrInfo> UpdateUserAsync(UsrInfo usrInfo, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Updating UsrInfo: {UsrId}", usrInfo.UsrId);

            string sSql = @"UPDATE UsrInfo SET DlrCode = @DlrCode, CoCode = @CoCode, CoBrchCode = @CoBrchCode, 
                           UsrName = @UsrName, UsrType = @UsrType, UsrNICNo = @UsrNICNo, UsrPassNo = @UsrPassNo,
                           UsrGender = @UsrGender, UsrDOB = @UsrDOB, UsrRace = @UsrRace, UsrEmail = @UsrEmail, 
                           UsrAddr = @UsrAddr, UsrPhone = @UsrPhone, UsrMobile = @UsrMobile, UsrFax = @UsrFax,
                           UsrStatus = @UsrStatus, UsrQualify = @UsrQualify, UsrRegisterDate = @UsrRegisterDate, 
                           UsrTDRDate = @UsrTDRDate, UsrResignDate = @UsrResignDate, ClntCode = @ClntCode,
                           UsrLicenseNo = @UsrLicenseNo, UsrExpiryDate = @UsrExpiryDate, RmsType = @RmsType, 
                           UsrSuperiorID = @UsrSuperiorID, UsrNotifierID = @UsrNotifierID, UsrLastUpdatedDate = @UsrLastUpdatedDate
                           WHERE UsrID = @UsrID";

            List<LB_DALParam> lstParam = CreateUsrInfoParameters(usrInfo, false);
            int rowsAffected = await _dal.LB_ExecuteNonQueryAsync(sSql, lstParam, CommandType.Text);

            if (rowsAffected == 0)
                throw new InvalidOperationException($"User with ID '{usrInfo.UsrId}' not found for update");

            _logger.LogDebug("Updated UsrInfo: {UsrId}, Rows affected: {RowsAffected}", usrInfo.UsrId, rowsAffected);
            return usrInfo;
        }

        public async Task<bool> DeleteUserAsync(string usrId, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Deleting UsrInfo: {UsrId}", usrId);

            string sSql = "DELETE FROM UsrInfo WHERE UsrID = @UsrID";
            List<LB_DALParam> lstParam = new List<LB_DALParam>
            {
                new LB_DALParam("UsrID", usrId)
            };

            int rowsAffected = await _dal.LB_ExecuteNonQueryAsync(sSql, lstParam, CommandType.Text);

            _logger.LogDebug("Deleted UsrInfo: {UsrId}, Rows affected: {RowsAffected}", usrId, rowsAffected);
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email, string? excludeUsrId = null, CancellationToken cancellationToken = default)
        {
            string sSql = @"SELECT COUNT(1) FROM UsrInfo WHERE UsrEmail = @Email 
                           AND (@ExcludeUsrId IS NULL OR UsrID != @ExcludeUsrId)";

            List<LB_DALParam> lstParam = new List<LB_DALParam>
            {
                new LB_DALParam("Email", email),
                new LB_DALParam("ExcludeUsrId", excludeUsrId ?? (object)DBNull.Value)
            };

            var count = Convert.ToInt32(await _dal.LB_ExecuteScalarAsync(sSql, lstParam, CommandType.Text));
            return count > 0;
        }

        public async Task<Dictionary<string, object>> GetUserStatisticsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Getting UsrInfo statistics");

            string sSql = @"SELECT COUNT(*) as TotalUsers,
                           SUM(CASE WHEN UsrStatus = 'A' THEN 1 ELSE 0 END) as ActiveUsers,
                           SUM(CASE WHEN UsrStatus = 'S' THEN 1 ELSE 0 END) as SuspendedUsers,
                           SUM(CASE WHEN UsrStatus = 'C' THEN 1 ELSE 0 END) as ClosedUsers
                           FROM UsrInfo";

            using DbDataReader dr = await _dal.LB_GetDbDataReaderAsync(sSql, null!, CommandType.Text);
            var stats = new Dictionary<string, object>();

            if (dr.Read())
            {
                stats["TotalUsers"] = Convert.ToInt32(dr["TotalUsers"]);
                stats["ActiveUsers"] = Convert.ToInt32(dr["ActiveUsers"]);
                stats["SuspendedUsers"] = Convert.ToInt32(dr["SuspendedUsers"]);
                stats["ClosedUsers"] = Convert.ToInt32(dr["ClosedUsers"]);
            }

            return stats;
        }

        #region Private Helper Methods - Following your existing patterns

        private UsrInfo MapUsrInfoFromReader(DbDataReader dr)
        {
            return new UsrInfo
            {
                UsrId = Convert.ToString(dr["UsrID"])!,
                DlrCode = dr["DlrCode"] == DBNull.Value ? null : Convert.ToString(dr["DlrCode"]),
                CoCode = dr["CoCode"] == DBNull.Value ? null : Convert.ToString(dr["CoCode"]),
                CoBrchCode = dr["CoBrchCode"] == DBNull.Value ? null : Convert.ToString(dr["CoBrchCode"]),
                UsrName = dr["UsrName"] == DBNull.Value ? null : Convert.ToString(dr["UsrName"]),
                UsrType = dr["UsrType"] == DBNull.Value ? null : Convert.ToInt32(dr["UsrType"]),
                UsrNicno = dr["UsrNICNo"] == DBNull.Value ? null : Convert.ToString(dr["UsrNICNo"]),
                UsrPassNo = dr["UsrPassNo"] == DBNull.Value ? null : Convert.ToString(dr["UsrPassNo"]),
                UsrGender = dr["UsrGender"] == DBNull.Value ? null : Convert.ToString(dr["UsrGender"]),
                UsrDob = dr["UsrDOB"] == DBNull.Value ? null : Convert.ToDateTime(dr["UsrDOB"]),
                UsrRace = dr["UsrRace"] == DBNull.Value ? null : Convert.ToString(dr["UsrRace"]),
                UsrEmail = dr["UsrEmail"] == DBNull.Value ? null : Convert.ToString(dr["UsrEmail"]),
                UsrAddr = dr["UsrAddr"] == DBNull.Value ? null : Convert.ToString(dr["UsrAddr"]),
                UsrPhone = dr["UsrPhone"] == DBNull.Value ? null : Convert.ToString(dr["UsrPhone"]),
                UsrMobile = dr["UsrMobile"] == DBNull.Value ? null : Convert.ToString(dr["UsrMobile"]),
                UsrFax = dr["UsrFax"] == DBNull.Value ? null : Convert.ToString(dr["UsrFax"]),
                UsrLastUpdatedDate = dr["UsrLastUpdatedDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["UsrLastUpdatedDate"]),
                UsrCreationDate = dr["UsrCreationDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["UsrCreationDate"]),
                UsrStatus = dr["UsrStatus"] == DBNull.Value ? null : Convert.ToString(dr["UsrStatus"]),
                UsrQualify = dr["UsrQualify"] == DBNull.Value ? null : Convert.ToString(dr["UsrQualify"]),
                UsrRegisterDate = dr["UsrRegisterDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["UsrRegisterDate"]),
                UsrTdrdate = dr["UsrTDRDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["UsrTDRDate"]),
                UsrResignDate = dr["UsrResignDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["UsrResignDate"]),
                ClntCode = dr["ClntCode"] == DBNull.Value ? null : Convert.ToString(dr["ClntCode"]),
                UsrLicenseNo = dr["UsrLicenseNo"] == DBNull.Value ? null : Convert.ToString(dr["UsrLicenseNo"]),
                UsrExpiryDate = dr["UsrExpiryDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["UsrExpiryDate"]),
                RmsType = Convert.ToString(dr["RmsType"])!,
                UsrSuperiorId = dr["UsrSuperiorID"] == DBNull.Value ? null : Convert.ToInt32(dr["UsrSuperiorID"]),
                UsrNotifierId = dr["UsrNotifierID"] == DBNull.Value ? null : Convert.ToInt32(dr["UsrNotifierID"])
            };
        }

        private List<LB_DALParam> CreateUsrInfoParameters(UsrInfo usrInfo, bool isInsert)
        {
            var lstParam = new List<LB_DALParam>
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
                new LB_DALParam("UsrNotifierID", usrInfo.UsrNotifierId ?? (object)DBNull.Value)
            };

            if (isInsert)
            {
                lstParam.Add(new LB_DALParam("UsrCreationDate", DateTime.UtcNow));
                usrInfo.UsrCreationDate = DateTime.UtcNow;
            }

            lstParam.Add(new LB_DALParam("UsrLastUpdatedDate", DateTime.UtcNow));
            usrInfo.UsrLastUpdatedDate = DateTime.UtcNow;

            return lstParam;
        }

        #endregion
    }
}