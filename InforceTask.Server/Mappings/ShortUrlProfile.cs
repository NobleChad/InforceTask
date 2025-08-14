using AutoMapper;
using InforceTask.Server.Models;
using InforceTask.Server.Models.DTOs;

namespace InforceTask.Server.Mappings
{
    public class ShortUrlProfile : Profile
    {
        public ShortUrlProfile()
        {
            CreateMap<ShortUrl, ShortUrlListDto>()
                .ForMember(dest => dest.ShortUrl,
                           opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.CanEdit,
                           opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ShortUrl, ShortUrlDetailDto>()
                .ForMember(dest => dest.ShortUrl,
                           opt => opt.MapFrom(src => string.Empty)) 
                .ReverseMap();
        }
    }
}
