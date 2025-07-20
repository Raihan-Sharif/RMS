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
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<RiskAssessment, RiskAssessmentDto>();

            CreateMap<AuditLog, AuditLogDto>();

            // UsrInfo mappings (simplified - no BaseEntity properties)
            CreateMap<UsrInfo, UsrInfoDto>();

            CreateMap<CreateUsrInfoRequest, UsrInfo>()
                .ForMember(dest => dest.UsrCreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.UsrLastUpdatedDate, opt => opt.Ignore());

            CreateMap<UpdateUsrInfoRequest, UsrInfo>()
                .ForMember(dest => dest.UsrId, opt => opt.Ignore()) // Don't update primary key
                .ForMember(dest => dest.UsrCreationDate, opt => opt.Ignore()) // Don't update creation date
                .ForMember(dest => dest.UsrLastUpdatedDate, opt => opt.Ignore()) // Will be set manually
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Only map non-null values
        }
    }
}