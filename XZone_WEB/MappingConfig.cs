using AutoMapper;
using XZone_WEB.Models;
using XZone_WEB.Models.DTO.CategoryDTo;
using XZone_WEB.Models.DTO.DeviceDTOs;
using XZone_WEB.Models.DTO.GameDTOs;
using XZone_WEB.Models.DTO.UserDto_s;

namespace XZone_WEB
{
    public class MappingConfig:Profile
    {

        public MappingConfig()
        {
            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<CategoryCreateDto, Category>().ReverseMap();
            CreateMap<CategoryUpdatedDTO, Category>().ReverseMap();

            CreateMap<Device, DeviceDTO>().ReverseMap();
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
