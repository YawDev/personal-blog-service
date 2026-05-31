using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalBlog.Api.ActionFilters;
using PersonalBlog.Api.Contracts.Request;
using PersonalBlog.Api.Contracts.Response.Auth;
using PersonalBlog.Api.Contracts.Response.Blogs;
using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
using PersonalBlog.Core.Interfaces;
using PersonalBlog.Models.DatabaseModels;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;

namespace PersonalBlog.Api.Controllers
{

    [ApiController]
    [Route("api")]
    public class AuthenticationController(IConfiguration configuration, IAuthenticationService authenticationService, IMapper mapper, SignInManager<ApplicationUser> signInManager, IUserIdentityService userIdentityService) : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IUserIdentityService _userIdentityService = userIdentityService;
        private readonly int _accessTokenExpirationMinutes = int.TryParse(configuration["Jwt:AccessTokenExpirationMinutes"], out var minutes) ? minutes : 30;
        private readonly int _refreshTokenExpirationDays = int.TryParse(configuration["Jwt:RefreshTokenExpirationDays"], out var days) ? days : 7;
        private readonly IMapper _mapper = mapper;

        [AllowAnonymous]
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var identityDTO = _mapper.Map<AuthenticateIdentityDTO>(request);
            var (user, accessToken, refreshToken) = await _authenticationService.AuthenticateUser(identityDTO);
            if (user == null)
            {
                return Unauthorized("Failed to authenticate credentials.");
            }

            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_accessTokenExpirationMinutes)
            });

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(_refreshTokenExpirationDays)
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
        /// Refreshes the access token using a valid refresh token cookie.
        /// Rotates the refresh token on each use — the old token is marked as used and a new one is issued.
        /// Returns 401 if the refresh token is missing, expired, revoked, or already used.
        /// </summary>
        /// <returns>Sets new access_token and refresh_token HttpOnly cookies on success</returns>
        [HttpPost("auth/refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            // Read refresh token cookies

            // Find in DB , validate isActive

            // mark old as isUsed = true

            //generate new access token + new refresh token

            //save new refresh token to DB

            //set both as HttpOnly cookies

            //return 200
            return Ok();
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
        
        /// <summary>
        /// Allows user to edit account details for profile
        /// </summary>
        /// <returns>Success or failure for account edit</returns>
        [Authorize]
        [HttpPut("account/edit/{id}")]
        public async Task<IActionResult> EditAccount(Guid id, [FromBody] EditAccountRequest editAccountRequest)
        {
            // Retrieve the pre-validated user from HttpContext
            var userId = HttpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")
            .Select(c => c.Value)
            .FirstOrDefault(v => Guid.TryParse(v, out _));

            if (userId == null || id != Guid.Parse(userId)) return Unauthorized();

            var user = await _authenticationService.GetIdentityUserAsync(Guid.Parse(userId));
            
            // Update the user's account details in the system
            var updateResult = await _userIdentityService.UpdateUserAsync(Guid.Parse(userId), _mapper.Map<EditAccountDTO>(editAccountRequest));
            
            return Ok(new EditAccountResponse()
            {
                IsUpdated = updateResult,
                UserGuid = Guid.Parse(userId)
            });        
        }
    }
}