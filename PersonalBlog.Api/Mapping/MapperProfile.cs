using AutoMapper;
using PersonalBlog.Api.Contracts.Request;
using PersonalBlog.Core.Dtos;
using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Api.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterRequest, CreateIdentityDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()));

            CreateMap<LoginRequest, AuthenticateIdentityDTO>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.Trim()));

            CreateMap<ApplicationUser, IdentityUserDTO>();

            CreateMap<BlogUser, BlogUserDTO>();
        }
    }
}