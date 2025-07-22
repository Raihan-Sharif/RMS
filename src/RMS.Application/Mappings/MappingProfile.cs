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
            // Domain entity mappings
            CreateMap<AuditLog, AuditLogDto>();

            // UsrInfo mappings - Map directly to/from domain entity
            CreateMap<UsrInfo, UsrInfoDto>().ReverseMap();

            CreateMap<CreateUsrInfoRequest, UsrInfo>()
                .ForMember(dest => dest.UsrCreationDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UsrLastUpdatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateUsrInfoRequest, UsrInfo>()
                .ForMember(dest => dest.UsrId, opt => opt.Ignore()) // Don't update primary key
                .ForMember(dest => dest.UsrCreationDate, opt => opt.Ignore()) // Don't update creation date
                .ForMember(dest => dest.UsrLastUpdatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Only map non-null values
        }
    }
}