using AutoMapper;
using PersonalBlog.Api.Contracts.Request;
using PersonalBlog.Api.Contracts.Response.Blogs;
using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.ResponseDtos;
using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Api.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region Request Models to DTOs
            CreateMap<RegisterRequest, CreateIdentityDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()));

            CreateMap<LoginRequest, AuthenticateIdentityDTO>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.Trim()));

            CreateMap<CreateBlogRequest, CreateBlogDTO>();
            #endregion

            #region DTOs to Response Models
            CreateMap<BlogResponseDTO, BlogResponse>();
            CreateMap<GetAllBlogsResponseDTO, GetAllBlogsResponse>();
            CreateMap<SaveBlogResponseDTO, SaveBlogResponse>();
            CreateMap<GetBlogByIdResponseDTO, GetBlogByIdResponse>();
            CreateMap<IdentityUserDTO, IdentityUserResponse>();            
            #endregion

            #region Database Models to DTOs
            CreateMap<BlogUser, BlogUserDTO>().ReverseMap();
            CreateMap<Post, PostDTO>().ReverseMap();
            CreateMap<Draft, DraftDTO>().ReverseMap();
            #endregion
            
            CreateMap<ApplicationUser, IdentityUserResponse>().ReverseMap();
            CreateMap<ApplicationUser, IdentityUserDTO>().ReverseMap();
            CreateMap<IdentityUserDTO, IdentityUserResponse>().ReverseMap();
        }
    }
}