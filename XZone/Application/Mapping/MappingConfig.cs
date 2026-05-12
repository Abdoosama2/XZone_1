using AutoMapper;
using XZone.Application.DTO.Category;
using XZone.Application.DTO.DeviceDTOs;
using XZone.Application.DTO.GameDTOs;
using XZone.Domain.Entites;
using XZone.Infrastructure.Identity;
using XZone.Models.DTO.UserDto_s;

namespace XZone.Application.Mapping
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<CategoryCreateDto, Category>().ReverseMap();
            CreateMap<CategoryUpdatedDTO, Category>().ReverseMap();
           
            CreateMap<Device,DeviceDTO>().ReverseMap();
            CreateMap<Device, DeviceCreateDTO>().ReverseMap();
            CreateMap<Device, DeviceUpdatedDTO>().ReverseMap();
            CreateMap<Game, GameDTO>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Game, GameUpdateDTO>().ReverseMap();
            CreateMap<Game, GameCreateDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserRegistrationDTO>().ReverseMap();

            //CreateMap<GameDevice,Game>
        }
    }
}
