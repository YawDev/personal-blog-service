using AutoMapper;
using PersonalBlog.Api.Contracts.Request;
using PersonalBlog.Api.Contracts.Response.Blogs;
using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
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
            CreateMap<SaveDraftRequest, SaveDraftDTO>();
            CreateMap<UpdateBlogRequest, PostDTO>()
                .ForMember(dest => dest.Userid, opt => opt.Ignore())
                .ForMember(dest => dest.Dateposted, opt => opt.Ignore())
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Lastmodifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
  
            CreateMap<EditAccountRequest, EditAccountDTO>();
            #endregion

            #region DTOs to Response Models
            CreateMap<BlogResponseDTO, BlogResponse>();
            CreateMap<GetAllBlogsResponseDTO, GetAllBlogsResponse>();
            CreateMap<SaveBlogResponseDTO, SaveBlogResponse>();
            CreateMap<GetBlogByIdResponseDTO, GetBlogByIdResponse>();
            CreateMap<IdentityUserDTO, IdentityUserResponse>();
            CreateMap<DeleteBlogResponseDTO, DeleteBlogResponse>();
            CreateMap<SendEmailResponseDTO, EmailSharelinkResponse>();
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