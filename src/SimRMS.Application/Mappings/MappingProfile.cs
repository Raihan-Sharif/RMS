using AutoMapper;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Entities;

namespace SimRMS.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for entity/DTO mappings
    /// SIMPLE APPROACH: Keep mapping straightforward, handle business logic in services
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // UsrInfo mappings - Bidirectional between Entity and DTO
            CreateMap<UsrInfo, UsrInfoDto>().ReverseMap();

            // ✅ SIMPLE: Request to Entity mapping (for CREATE)
            CreateMap<UsrInfoRequest, UsrInfo>()
                .ForMember(dest => dest.UsrCreationDate, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.UsrLastUpdatedDate, opt => opt.Ignore()) // Set in service
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // DTO to Request mappings (for reverse scenarios)
            CreateMap<UsrInfoDto, UsrInfoRequest>();

            // Additional entity mappings
            // CreateMap<OtherEntity, OtherEntityDto>().ReverseMap();
        }
    }

    /// <summary>
    /// Extension methods for mapping operations
    /// Business logic handled here instead of complex AutoMapper configurations
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// Maps UsrInfoRequest to UsrInfo for CREATE operation
        /// Sets appropriate defaults and timestamps
        /// </summary>
        public static UsrInfo ToEntityForCreate(this UsrInfoRequest request, IMapper mapper)
        {
            var entity = mapper.Map<UsrInfo>(request);

            // Set CREATE-specific values
            entity.UsrCreationDate = DateTime.UtcNow;
            entity.UsrLastUpdatedDate = DateTime.UtcNow;

            // Set defaults if not provided
            entity.UsrStatus = !string.IsNullOrEmpty(request.UsrStatus) ? request.UsrStatus : "A";
            entity.RmsType = !string.IsNullOrEmpty(request.RmsType) ? request.RmsType : throw new ArgumentException("RmsType is required for create");
            entity.UsrId = !string.IsNullOrEmpty(request.UsrId) ? request.UsrId : throw new ArgumentException("UsrId is required for create");

            return entity;
        }

        /// <summary>
        /// Updates existing entity from request for UPDATE operation
        /// Only updates non-null properties, preserves existing data
        /// </summary>
        public static void UpdateFromRequest(this UsrInfo existingEntity, UsrInfoRequest request, IMapper mapper)
        {
            // Store values that should never be updated
            var originalUsrId = existingEntity.UsrId;
            var originalCreationDate = existingEntity.UsrCreationDate;

            // Map non-null values from request
            if (!string.IsNullOrEmpty(request.DlrCode)) existingEntity.DlrCode = request.DlrCode;
            if (!string.IsNullOrEmpty(request.CoCode)) existingEntity.CoCode = request.CoCode;
            if (!string.IsNullOrEmpty(request.CoBrchCode)) existingEntity.CoBrchCode = request.CoBrchCode;
            if (!string.IsNullOrEmpty(request.UsrName)) existingEntity.UsrName = request.UsrName;
            if (request.UsrType.HasValue) existingEntity.UsrType = request.UsrType;
            if (!string.IsNullOrEmpty(request.UsrNicno)) existingEntity.UsrNicno = request.UsrNicno;
            if (!string.IsNullOrEmpty(request.UsrPassNo)) existingEntity.UsrPassNo = request.UsrPassNo;
            if (!string.IsNullOrEmpty(request.UsrGender)) existingEntity.UsrGender = request.UsrGender;
            if (request.UsrDob.HasValue) existingEntity.UsrDob = request.UsrDob;
            if (!string.IsNullOrEmpty(request.UsrRace)) existingEntity.UsrRace = request.UsrRace;
            if (!string.IsNullOrEmpty(request.UsrEmail)) existingEntity.UsrEmail = request.UsrEmail;
            if (!string.IsNullOrEmpty(request.UsrAddr)) existingEntity.UsrAddr = request.UsrAddr;
            if (!string.IsNullOrEmpty(request.UsrPhone)) existingEntity.UsrPhone = request.UsrPhone;
            if (!string.IsNullOrEmpty(request.UsrMobile)) existingEntity.UsrMobile = request.UsrMobile;
            if (!string.IsNullOrEmpty(request.UsrFax)) existingEntity.UsrFax = request.UsrFax;
            if (!string.IsNullOrEmpty(request.UsrStatus)) existingEntity.UsrStatus = request.UsrStatus;
            if (!string.IsNullOrEmpty(request.UsrQualify)) existingEntity.UsrQualify = request.UsrQualify;
            if (request.UsrRegisterDate.HasValue) existingEntity.UsrRegisterDate = request.UsrRegisterDate;
            if (request.UsrTdrdate.HasValue) existingEntity.UsrTdrdate = request.UsrTdrdate;
            if (request.UsrResignDate.HasValue) existingEntity.UsrResignDate = request.UsrResignDate;
            if (!string.IsNullOrEmpty(request.ClntCode)) existingEntity.ClntCode = request.ClntCode;
            if (!string.IsNullOrEmpty(request.UsrLicenseNo)) existingEntity.UsrLicenseNo = request.UsrLicenseNo;
            if (request.UsrExpiryDate.HasValue) existingEntity.UsrExpiryDate = request.UsrExpiryDate;
            if (!string.IsNullOrEmpty(request.RmsType)) existingEntity.RmsType = request.RmsType;
            if (request.UsrSuperiorId.HasValue) existingEntity.UsrSuperiorId = request.UsrSuperiorId;
            if (request.UsrNotifierId.HasValue) existingEntity.UsrNotifierId = request.UsrNotifierId;
            if (!string.IsNullOrEmpty(request.Category)) existingEntity.Category = request.Category;
            if (!string.IsNullOrEmpty(request.UsrChannel)) existingEntity.UsrChannel = request.UsrChannel;
            if (!string.IsNullOrEmpty(request.Pid)) existingEntity.Pid = request.Pid;
            if (!string.IsNullOrEmpty(request.PidRms)) existingEntity.PidRms = request.PidRms;

            // Always preserve these values and update timestamp
            existingEntity.UsrId = originalUsrId;
            existingEntity.UsrCreationDate = originalCreationDate;
            existingEntity.UsrLastUpdatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Generic entity to DTO converter
        /// </summary>
        public static TDto ToDto<TDto>(this object entity, IMapper mapper) where TDto : class
        {
            return mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Generic collection to DTOs converter
        /// </summary>
        public static IEnumerable<TDto> ToDtos<TDto>(this IEnumerable<object> entities, IMapper mapper) where TDto : class
        {
            return mapper.Map<IEnumerable<TDto>>(entities);
        }
    }
}