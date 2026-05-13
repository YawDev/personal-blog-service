using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalBlog.Api.ActionFilters;
using PersonalBlog.Api.Contracts.Request;
using PersonalBlog.Api.Contracts.Response.Auth;
using PersonalBlog.Api.Contracts.Response.Blogs;
using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Interfaces;
using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;
using System.Security.Claims;

namespace PersonalBlog.Api.Controllers
{

    [ApiController]
    [Route("api")]
    public class AuthenticationController(IAuthenticationService authenticationService, IMapper mapper, SignInManager<ApplicationUser> signInManager) : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IMapper _mapper = mapper;

        [AllowAnonymous]
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var identityDTO = _mapper.Map<AuthenticateIdentityDTO>(request);
            var (user, token) = await _authenticationService.AuthenticateUser(identityDTO);
            if (user == null)
            {
                return Unauthorized("Failed to authenticate credentials.");
            }
            
            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });
            
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok(new LoginResponse()
            {
                    User = _mapper.Map<IdentityUserResponse>(user)
            });
        }

        [AllowAnonymous]
        [HttpPost("auth/register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var identityDTO = _mapper.Map<CreateIdentityDTO>(request);
            await _authenticationService.CreateUserAndIdentity(identityDTO);
            return Ok("User registered successfully");
        }

        [HttpPost("auth/logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("User logged out successfully");
        }

        
        /// <summary>
        /// Retrieves user information for the authenticated user.
        /// The id is passed as a parameter and used to retrieve data for the signed-in user.
        /// </summary>
        /// <param name="id">The unique identifier of the identity user</param>
        /// <returns>User information for the authenticated user</returns>
        [IdentityFilter]
        [HttpGet("auth/user/{id}")]
        public async Task<IActionResult> GetUserInfo(Guid id)
        {
            var user = await _authenticationService.GetUserByIdAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Retrieves identity information for the authenticated user.
        /// The id is passed as a parameter and used to retrieve data for the signed-in user.
        /// </summary>
        /// <param name="id">The unique identifier of the identity user</param>
        /// <returns>Identity information for the authenticated user</returns>
        [IdentityFilter]
        [HttpGet("auth/identity/{id}")]
        public async Task<IActionResult> GetIdentityInfo(Guid id)
        {
            // Retrieve the pre-validated user from HttpContext
            var identityUser = HttpContext.Items["AuthenticatedIdentity"] as IdentityUserDTO;
            return Ok(identityUser);
        }

        /// <summary>
        /// Retrieves identity information for the authenticated user.
        /// </summary>
        /// <returns>Identity information for the authenticated user</returns>
        [Authorize]
        [HttpGet("auth/me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Retrieve the pre-validated user from HttpContext
            var userId = HttpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")
            .Select(c => c.Value)
            .FirstOrDefault(v => Guid.TryParse(v, out _));

            if (userId == null) return Unauthorized();

            var user = await _authenticationService.GetIdentityUserAsync(Guid.Parse(userId));
            return Ok(new CheckIdentityResponse()
            {
                User = _mapper.Map<IdentityUserResponse>(user)
            });        
        }
    }
}