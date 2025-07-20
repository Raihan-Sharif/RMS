using RMS.Domain.Entities;
using RMS.Domain.Interfaces;
using RMS.Infrastructure.Data.Entities;
using System.Collections.Concurrent;

namespace RMS.Infrastructure.Mappings
{
    public class EntityMappingRegistry : IEntityMapper
    {
        private readonly ConcurrentDictionary<Type, EntityMappingInfo> _mappings = new();

        public EntityMappingRegistry()
        {
            RegisterMappings();
        }

        private void RegisterMappings()
        {
            // Register all your entity mappings here
            // This will be populated after scaffolding all entities

            // Example registration - add more as you scaffold entities
            RegisterEntity<UsrInfo, UsrInfoDbEntity>(
                tableName: "UsrInfo",
                primaryKeyName: "UsrId",
                primaryKeyType: typeof(string)
            );

            // Add more entities as they are scaffolded:
            // RegisterEntity<Customer, CustomerDbEntity>("Customer", "CustomerId", typeof(string));
            // RegisterEntity<Order, OrderDbEntity>("Orders", "OrderId", typeof(int));
            // etc...
        }

        private void RegisterEntity<TDomain, TDbEntity>(string tableName, string primaryKeyName, Type primaryKeyType)
            where TDomain : class
            where TDbEntity : class
        {
            _mappings[typeof(TDomain)] = new EntityMappingInfo
            {
                DomainType = typeof(TDomain),
                DbEntityType = typeof(TDbEntity),
                TableName = tableName,
                PrimaryKeyName = primaryKeyName,
                PrimaryKeyType = primaryKeyType
            };
        }

        public Type GetDbEntityType(Type domainEntityType)
        {
            return _mappings.TryGetValue(domainEntityType, out var mapping)
                ? mapping.DbEntityType
                : throw new InvalidOperationException($"No mapping registered for {domainEntityType.Name}");
        }

        public Type GetDomainEntityType(Type dbEntityType)
        {
            var mapping = _mappings.Values.FirstOrDefault(m => m.DbEntityType == dbEntityType);
            return mapping?.DomainType ?? throw new InvalidOperationException($"No domain mapping found for {dbEntityType.Name}");
        }

        public string GetTableName(Type domainEntityType)
        {
            return _mappings.TryGetValue(domainEntityType, out var mapping)
                ? mapping.TableName
                : throw new InvalidOperationException($"No table name registered for {domainEntityType.Name}");
        }

        public string GetPrimaryKeyName(Type domainEntityType)
        {
            return _mappings.TryGetValue(domainEntityType, out var mapping)
                ? mapping.PrimaryKeyName
                : "Id";
        }

        public Type GetPrimaryKeyType(Type domainEntityType)
        {
            return _mappings.TryGetValue(domainEntityType, out var mapping)
                ? mapping.PrimaryKeyType
                : typeof(Guid);
        }

        public bool IsRegistered(Type domainEntityType)
        {
            return _mappings.ContainsKey(domainEntityType);
        }

        private class EntityMappingInfo
        {
            public Type DomainType { get; set; } = null!;
            public Type DbEntityType { get; set; } = null!;
            public string TableName { get; set; } = null!;
            public string PrimaryKeyName { get; set; } = null!;
            public Type PrimaryKeyType { get; set; } = null!;
        }
    }
}