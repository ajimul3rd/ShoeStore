using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using ShoeStore.Model.Entity;
using ShoeStore.Shared.Dto;
using OfficeProject.Authentication;
using ShoeStore.Shared.Enums;
using ShoeStore.Servicess.Impl;
using System.Linq;
using System.Net;
namespace ShoeStore.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper Mapper;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;

        public AuthController(
            IUserService userService,
            ITokenService tokenService,
            IPasswordService passwordService,
            IConfiguration configuration,
            IMapper mapper)
        {
            this._userService = userService;
            this._tokenService = tokenService;
            this._passwordService = passwordService;
            this._configuration = configuration;
            this.Mapper = mapper;
        }

        [HttpPost("register-client")]
        public async Task<IActionResult> ClientRegister([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userService.FindUserByUsername(model.UserName!) != null)
                return Conflict("Username already exists");

            var user = new Users
            {
                UserName = model.UserName,
                UserEmail = model.UserEmail,
                UserContact = model.UserContact,
                UserPassword = _passwordService.HashPassword(model.UserPassword!),
                Role = UserRole.CLIENT,
                Address = model.Address,
                IsActive = true
            };

            try
            {
                await _userService.AddUserAsync(user);
                // Generate claims identical to login
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName!),
                    new(ClaimTypes.Role, user.Role.ToString()!),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Email, user.UserEmail !),
                    new("Contact", user.UserContact !)
                };

                var token = GenerateJwtToken(claims);
                var refreshToken = _tokenService.GenerateRefreshToken();

                await _userService.UpdateRefreshToken(user.UserId, refreshToken);

                return Ok(new AuthResponse
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    ExpiresIn = 43200,
                    UserInfo = new UserInfoDto
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        UserEmail = user.UserEmail,
                        Role = user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Registration failed: {ex.Message}");
            }
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> AdminRegister([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userService.FindUserByUsername(model.UserName!) != null)
                return Conflict("Username already exists");

            var user = new Users
            {
                UserName = model.UserName,
                UserEmail = model.UserEmail,
                UserContact = model.UserContact,
                UserPassword = _passwordService.HashPassword(model.UserPassword!),
                Role = UserRole.ADMIN,
                IsActive = true
            };

            try
            {
                await _userService.AddUserAsync(user);
                // Generate claims identical to login
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName!),
                    new(ClaimTypes.Role, user.Role.ToString()!),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Email, user.UserEmail !),
                    new("Contact", user.UserContact !)
                };

                var token = GenerateJwtToken(claims);
                var refreshToken = _tokenService.GenerateRefreshToken();

                await _userService.UpdateRefreshToken(user.UserId, refreshToken);

                return Ok(new AuthResponse
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    ExpiresIn = 43200,
                    UserInfo = new UserInfoDto
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        UserEmail = user.UserEmail,
                        Role = user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Registration failed: {ex.Message}");
            }
        }


        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginModel login)
        {
            if (login == null)
                return BadRequest("Login data is required");

            var user = await _userService.FindUserByUsername(login.UserName!);

            if (user == null || !_passwordService.VerifyPassword(login.UserPassword!, user.UserPassword!))
                return Unauthorized("Invalid credentials");

            if (!user.IsActive)
                return Unauthorized("Account disabled");

            var claims = GenerateUserClaims(Mapper.Map<Users>(user));
            var token = GenerateJwtToken(claims); // Returns JwtSecurityToken
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _userService.UpdateRefreshToken(Mapper.Map<Users>(user).UserId, refreshToken);
            return Ok(new AuthResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresIn = (int)TimeSpan.FromHours(12).TotalSeconds,
                UserInfo = new UserInfoDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    UserEmail = user.UserEmail,
                    Role = (UserRole)user.Role!
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UsersDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid user data.");

            var user = await _userService.FindUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            // Map DTO -> Entity
            user.UserName = dto.UserName ?? user.UserName;
            user.UserEmail = dto.UserEmail ?? user.UserEmail;
            user.UserContact = dto.UserContact ?? user.UserContact;
            if (dto.ShippingAddressDto != null)
            {
                user.Address = Mapper.Map<List<ShippingAddress>>(dto.ShippingAddressDto) ?? new List<ShippingAddress>();
            }
            if (dto.IsActive)
                user.IsActive = dto.IsActive;

            var success = await _userService.UpdateUserAsync(user);
            if (!success)
                return StatusCode(500, "Failed to update user. Please try again later.");

            return Ok(new { message = "User updated successfully", userId = user.UserId });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(new { Message = "User deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // log exception
                return StatusCode(500, new { Message = "An error occurred while deleting the user", Details = ex.Message });
            }
        }



        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenModel model)
        {
            try
            {
                // 1. Validate input
                if (model?.AccessToken == null || model.RefreshToken == null)
                    return BadRequest(new { Message = "Tokens are required" });

                // 2. Get principal from expired token
                var principal = _tokenService.GetPrincipalFromExpiredToken(model.AccessToken);
                if (principal?.Identity?.Name == null)
                    return BadRequest(new { Message = "Invalid token" });

                // 3. Get user from database
                var user = await _userService.FindUserByUsername(principal.Identity.Name);
                if (user == null)
                    return Unauthorized(new { Message = "User not found" });

                // 4. Validate refresh token
                if (Mapper.Map<Users>(user).RefreshToken != model.RefreshToken || Mapper.Map<Users>(user).RefreshTokenExpiry <= DateTime.UtcNow)
                    return Unauthorized(new { Message = "Invalid refresh token" });

                // 5. Generate new tokens (with null checks)
                var newAccessToken = GenerateJwtToken(principal.Claims);
                if (string.IsNullOrEmpty(newAccessToken))
                    return StatusCode(500, new { Message = "Failed to generate access token" });

                var newRefreshToken = _tokenService.GenerateRefreshToken();
                if (string.IsNullOrEmpty(newRefreshToken))
                    return StatusCode(500, new { Message = "Failed to generate refresh token" });

                // 6. Update refresh token in database
                await _userService.UpdateRefreshToken(Mapper.Map<Users>(user).UserId, newRefreshToken);

                return Ok(new AuthResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresIn = (int)TimeSpan.FromHours(12).TotalSeconds
                });
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { Message = "Invalid token", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during token refresh", Details = ex.Message });
            }
        }


        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var secret = _configuration["JwtSettings:Secret"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];

            //var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
            //var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            //var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

            if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                throw new InvalidOperationException("JWT configuration is missing in environment variables.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private List<Claim> GenerateUserClaims(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName !),
                new Claim(ClaimTypes.Role, user.Role.ToString()!),
                new Claim(JwtRegisteredClaimNames.Email, user.UserEmail !),
                new Claim("Contact", user.UserContact !),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            if (username != null)
            {
                await _userService.RevokeRefreshToken(username);
            }
            return Ok("Logged out successfully");
        }
    }

}

