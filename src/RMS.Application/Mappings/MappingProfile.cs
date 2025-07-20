using AutoMapper;
using RMS.Application.Models.DTOs;
using RMS.Application.Models.Requests;
using RMS.Domain.Entities;

namespace RMS.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain entity mappings (existing)
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<RiskAssessment, RiskAssessmentDto>();
            CreateMap<AuditLog, AuditLogDto>();

            // Infrastructure entity mappings (scaffolded entities)
            //CreateMap<Infrastructure.Data.Entities.UsrInfo, UsrInfoDto>();

            //CreateMap<CreateUsrInfoRequest, Infrastructure.Data.Entities.UsrInfo>()
            //    .ForMember(dest => dest.UsrCreationDate, opt => opt.Ignore())
            //    .ForMember(dest => dest.UsrLastUpdatedDate, opt => opt.Ignore());

            //CreateMap<UpdateUsrInfoRequest, Infrastructure.Data.Entities.UsrInfo>()
            //    .ForMember(dest => dest.UsrId, opt => opt.Ignore()) // Don't update primary key
            //    .ForMember(dest => dest.UsrCreationDate, opt => opt.Ignore()) // Don't update creation date
            //    .ForMember(dest => dest.UsrLastUpdatedDate, opt => opt.Ignore()) // Will be set automatically
            //    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Only map non-null values
        }
    }
}
