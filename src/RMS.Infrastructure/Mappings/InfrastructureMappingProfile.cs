using AutoMapper;
using RMS.Domain.Entities;
using RMS.Infrastructure.Data.Entities;

namespace RMS.Infrastructure.Mappings
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            // Auto-mapping for all entities - will be discovered dynamically
            CreateDynamicMappings();
        }

        private void CreateDynamicMappings()
        {
            // Get all domain entities
            var domainAssembly = typeof(UsrInfo).Assembly;
            var domainEntityTypes = domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "RMS.Domain.Entities")
                .ToList();

            // Get all database entities
            var infrastructureAssembly = typeof(UsrInfoDbEntity).Assembly;
            var dbEntityTypes = infrastructureAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "RMS.Infrastructure.Data.Entities" && t.Name.EndsWith("DbEntity"))
                .ToList();

            // Create mappings between matching entities
            foreach (var domainType in domainEntityTypes)
            {
                var correspondingDbType = dbEntityTypes.FirstOrDefault(db =>
                    db.Name == domainType.Name + "DbEntity");

                if (correspondingDbType != null)
                {
                    // Create bidirectional mapping
                    CreateMap(domainType, correspondingDbType).ReverseMap();
                }
            }
        }
    }
}