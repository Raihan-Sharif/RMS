using Microsoft.Extensions.Logging;
using SimRMS.Domain.Entities;
using SimRMS.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Reflection;
using LB.DAL.Core.Common;

namespace SimRMS.Infrastructure.Data
{
    /// <summary>
    /// Infrastructure implementation of entity mapping
    /// Contains LB.DAL specific mapping logic
    /// </summary>
    public class LBEntityMapper : IEntityMapper
    {
        private readonly ILogger<LBEntityMapper> _logger;
        private readonly Dictionary<Type, EntityMetadata> _entityMetadata;

        public LBEntityMapper(ILogger<LBEntityMapper> logger)
        {
            _logger = logger;
            _entityMetadata = new Dictionary<Type, EntityMetadata>();
            InitializeEntityMetadata();
        }

        #region Domain Interface Implementation

        public string GetPrimaryKeyName<T>() where T : class
        {
            var type = typeof(T);

            if (_entityMetadata.TryGetValue(type, out var metadata))
            {
                return metadata.PrimaryKeyColumn;
            }

            // Default fallback
            if (type == typeof(UsrInfo)) return "UsrID";
            if (type == typeof(UsrLogin)) return "UsrID";
            if (type == typeof(AuditLog)) return "Id";

            return "Id";
        }

        public string GetTableName<T>() where T : class
        {
            var type = typeof(T);

            if (_entityMetadata.TryGetValue(type, out var metadata))
            {
                return metadata.TableName;
            }

            // Default fallback
            return type.Name;
        }

        public TKey? GetEntityId<TKey, TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                var type = typeof(TEntity);
                var primaryKeyName = GetPrimaryKeyName<TEntity>();

                var property = type.GetProperty(primaryKeyName);
                if (property != null)
                {
                    var value = property.GetValue(entity);
                    return (TKey?)value;
                }
                return default(TKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entity ID for {EntityType}", typeof(TEntity).Name);
                return default(TKey);
            }
        }

        #endregion

        #region Infrastructure-Specific Methods

        /// <summary>
        /// Maps a single data reader record to an entity
        /// </summary>
        public T MapToEntity<T>(DbDataReader reader) where T : class, new()
        {
            var entity = new T();
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (!property.CanWrite) continue;

                var columnName = GetColumnName(property);

                try
                {
                    if (HasColumn(reader, columnName))
                    {
                        var value = reader[columnName];
                        if (value != DBNull.Value)
                        {
                            var convertedValue = ConvertValue(value, property.PropertyType);
                            property.SetValue(entity, convertedValue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to map column {ColumnName} to property {PropertyName} for type {TypeName}",
                        columnName, property.Name, type.Name);
                }
            }

            return entity;
        }

        /// <summary>
        /// Maps multiple data reader records to a list of entities
        /// </summary>
        public async Task<List<T>> MapToListAsync<T>(DbDataReader reader) where T : class, new()
        {
            var results = new List<T>();

            while (await reader.ReadAsync())
            {
                results.Add(MapToEntity<T>(reader));
            }

            return results;
        }

        /// <summary>
        /// Maps data reader to a dictionary for dynamic access
        /// </summary>
        public Dictionary<string, object> MapToDictionary(DbDataReader reader)
        {
            var result = new Dictionary<string, object>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var value = reader.GetValue(i);
                result[columnName] = value == DBNull.Value ? null! : value;
            }

            return result;
        }

        /// <summary>
        /// Creates LB_DALParam from entity properties
        /// </summary>
        public List<LB_DALParam> CreateParametersFromEntity<T>(T entity, string[]? excludeProperties = null) where T : class
        {
            var parameters = new List<LB_DALParam>();
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var excludeSet = excludeProperties?.ToHashSet() ?? new HashSet<string>();

            foreach (var property in properties)
            {
                if (!property.CanRead || excludeSet.Contains(property.Name)) continue;

                var columnName = GetColumnName(property);
                var value = property.GetValue(entity);

                parameters.Add(new LB_DALParam(columnName, value ?? DBNull.Value));
            }

            return parameters;
        }

        #endregion

        #region Private Helper Methods

        private void InitializeEntityMetadata()
        {
            // UsrInfo metadata
            _entityMetadata[typeof(UsrInfo)] = new EntityMetadata
            {
                TableName = "UsrInfo",
                PrimaryKeyColumn = "UsrID",
                Columns = new Dictionary<string, string>
                {
                    { "UsrId", "UsrID" },
                    { "DlrCode", "DlrCode" },
                    { "CoCode", "CoCode" },
                    { "CoBrchCode", "CoBrchCode" },
                    { "UsrName", "UsrName" },
                    { "UsrType", "UsrType" },
                    { "UsrNicno", "UsrNICNo" },
                    { "UsrPassNo", "UsrPassNo" },
                    { "UsrGender", "UsrGender" },
                    { "UsrDob", "UsrDOB" },
                    { "UsrRace", "UsrRace" },
                    { "UsrEmail", "UsrEmail" },
                    { "UsrAddr", "UsrAddr" },
                    { "UsrPhone", "UsrPhone" },
                    { "UsrMobile", "UsrMobile" },
                    { "UsrFax", "UsrFax" },
                    { "UsrLastUpdatedDate", "UsrLastUpdatedDate" },
                    { "UsrCreationDate", "UsrCreationDate" },
                    { "UsrStatus", "UsrStatus" },
                    { "UsrQualify", "UsrQualify" },
                    { "UsrRegisterDate", "UsrRegisterDate" },
                    { "UsrTdrdate", "UsrTDRDate" },
                    { "UsrResignDate", "UsrResignDate" },
                    { "ClntCode", "ClntCode" },
                    { "UsrLicenseNo", "UsrLicenseNo" },
                    { "UsrExpiryDate", "UsrExpiryDate" },
                    { "RmsType", "RmsType" },
                    { "UsrSuperiorId", "UsrSuperiorID" },
                    { "UsrNotifierId", "UsrNotifierID" }
                }
            };

            // UsrLogin metadata
            _entityMetadata[typeof(UsrLogin)] = new EntityMetadata
            {
                TableName = "UsrLogin",
                PrimaryKeyColumn = "UsrID",
                Columns = new Dictionary<string, string>
                {
                    { "UsrId", "UsrID" },
                    { "UsrPwd", "UsrPwd" },
                    { "UsrPwd1", "UsrPwd1" },
                    { "UsrPwdUnscsAtmpt", "UsrPwdUnscsAtmpt" },
                    { "UsrPwdLastChgDate", "UsrPwdLastChgDate" },
                    { "UsrDisableWrngDate", "UsrDisableWrngDate" },
                    { "UsrTrdgPin", "UsrTrdgPin" },
                    { "UsrTrdgPinUnscsAtmpt", "UsrTrdgPinUnscsAtmpt" },
                    { "UsrTrdgPinStat", "UsrTrdgPinStat" },
                    { "UsrTrdgPinLastChgDate", "UsrTrdgPinLastChgDate" },
                    { "UsrTrdgPinDisableWrngDate", "UsrTrdgPinDisableWrngDate" },
                    { "UsrLogin1", "UsrLogin" },
                    { "UsrActiveTime", "UsrActiveTime" },
                    { "UsrLastUpdatedDate", "UsrLastUpdatedDate" },
                    { "UsrPwdReset", "UsrPwdReset" },
                    { "UsrActvnCode", "UsrActvnCode" },
                    { "UsrSecretAns1", "UsrSecretAns1" },
                    { "UsrSecretAns2", "UsrSecretAns2" },
                    { "UsrSecretAns3", "UsrSecretAns3" },
                    { "UsrForceLogout", "UsrForceLogout" },
                    { "UsrActvCode", "UsrActvCode" },
                    { "UsrLastLoginDate", "UsrLastLoginDate" },
                    { "UsrTrdgPinReset", "UsrTrdgPinReset" },
                    { "UsrOtpresendAtt", "UsrOTPResendAtt" },
                    { "UsrOtpvldtAtt", "UsrOTPVldtAtt" },
                    { "UsrOtpcode", "UsrOTPCode" },
                    { "UsrOtpexpiration", "UsrOTPExpiration" },
                    { "UsrTwoFactorAuth", "UsrTwoFactorAuth" },
                    { "UsrTwoFactorAuthBypassExpiryDate", "UsrTwoFactorAuthBypassExpiryDate" }
                }
            };

            // AuditLog metadata
            _entityMetadata[typeof(AuditLog)] = new EntityMetadata
            {
                TableName = "AuditLog",
                PrimaryKeyColumn = "Id",
                Columns = new Dictionary<string, string>() // Add if needed
            };
        }

        private string GetColumnName(PropertyInfo property)
        {
            var type = property.DeclaringType;

            if (type != null && _entityMetadata.TryGetValue(type, out var metadata)
                && metadata.Columns.TryGetValue(property.Name, out var columnName))
            {
                return columnName;
            }

            // Check for Column attribute
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.Name))
            {
                return columnAttribute.Name;
            }

            return property.Name;
        }

        private bool HasColumn(DbDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return ordinal >= 0;
            }
            catch
            {
                return false;
            }
        }

        private object? ConvertValue(object value, Type targetType)
        {
            if (value == null || value == DBNull.Value) return null;

            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType == typeof(bool) && value is int intValue)
            {
                return intValue != 0;
            }

            if (underlyingType.IsEnum)
            {
                return Enum.ToObject(underlyingType, value);
            }

            return Convert.ChangeType(value, underlyingType);
        }

        private class EntityMetadata
        {
            public string TableName { get; set; } = string.Empty;
            public string PrimaryKeyColumn { get; set; } = string.Empty;
            public Dictionary<string, string> Columns { get; set; } = new();
        }

        #endregion
    }
}
