using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using ShoeStore.Model.Entity;
using ShoeStore.Shared.Dto;
using ShoeStore.Auth;
using ShoeStore.Shared.Enums;
using ShoeStore.Servicess.Impl;
using log4net;
namespace ShoeStore.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AuthController));
       

        public AuthController(
            IUserService userService,
            ITokenService tokenService,
            IPasswordService passwordService,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userService = userService;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _configuration = configuration;
            _mapper = mapper;
         

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
            try
            {
                if (login == null)
                    return BadRequest("Login data is required");

                var user = await _userService.FindUserByUsername(login.UserName!);

                if (user == null || !_passwordService.VerifyPassword(login.UserPassword!, user.UserPassword!))
                    return Unauthorized("Invalid credentials");

                if (!user.IsActive)
                    return Unauthorized("Account disabled");

                var claims = GenerateUserClaims(_mapper.Map<Users>(user));
                var token = GenerateJwtToken(claims); 
                var refreshToken = _tokenService.GenerateRefreshToken();

                await _userService.UpdateRefreshToken(_mapper.Map<Users>(user).UserId, refreshToken);

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
            catch (Exception ex)
            {
                _logger.Error("Error occurred during login.", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UsersDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Invalid user data.");

                var user = await _userService.FindUserByIdAsync(id);
                if (user == null)
                    return NotFound("User not found");
                user.UserName = dto.UserName ?? user.UserName;
                user.UserEmail = dto.UserEmail ?? user.UserEmail;
                user.UserContact = dto.UserContact ?? user.UserContact;

                if (dto.ShippingAddressDto != null)
                {
                    user.Address = _mapper.Map<List<ShippingAddress>>(dto.ShippingAddressDto) ?? new List<ShippingAddress>();
                }

                if (dto.IsActive)
                {
                    user.IsActive = dto.IsActive;
                }

                var success = await _userService.UpdateUserAsync(user);
                if (!success)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update user. Please try again later.");

                return Ok(new { message = "User updated successfully", userId = user.UserId });
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while updating user.",ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while updating the user.");
            }
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
                if (_mapper.Map<Users>(user).RefreshToken != model.RefreshToken || _mapper.Map<Users>(user).RefreshTokenExpiry <= DateTime.UtcNow)
                    return Unauthorized(new { Message = "Invalid refresh token" });

                // 5. Generate new tokens (with null checks)
                var newAccessToken = GenerateJwtToken(principal.Claims);
                if (string.IsNullOrEmpty(newAccessToken))
                    return StatusCode(500, new { Message = "Failed to generate access token" });

                var newRefreshToken = _tokenService.GenerateRefreshToken();
                if (string.IsNullOrEmpty(newRefreshToken))
                    return StatusCode(500, new { Message = "Failed to generate refresh token" });

                // 6. Update refresh token in database
                await _userService.UpdateRefreshToken(_mapper.Map<Users>(user).UserId, newRefreshToken);

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
            try
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
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate JWT token.", ex);
            }
        }


        private List<Claim> GenerateUserClaims(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                return new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                        new Claim(ClaimTypes.Role, user.Role?.ToString() ?? string.Empty),
                        new Claim(JwtRegisteredClaimNames.Email, user.UserEmail ?? string.Empty),
                        new Claim("Contact", user.UserContact ?? string.Empty),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate user claims.", ex);
            }
        }


        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var username = User?.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                await _userService.RevokeRefreshToken(username);
                return Ok("Logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during logout for user {username}", ex);
                return StatusCode(500, "An error occurred during logout.");
            }
        }
    }

}

