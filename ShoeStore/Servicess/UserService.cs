using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OfficeProject.Authentication;
using ShoeStore.Data;
using ShoeStore.Model.Entity;
using ShoeStore.Servicess.Impl;
using ShoeStore.Shared.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShoeStore.Servicess
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Users> _passwordHasher;

        public UserService(
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<Users>();
        }

        public async Task<Users?>FindUserByUsername(string username)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                // Find the user by the provided id (not user.UserId)
                return await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            }
        }
        public async Task<Users?> FindUserByIdAsync(int id)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            }
        }
        public async Task AddUserAsync(Users user)
        {
            try
            {
                using (var context = _dbContextFactory.CreateDbContext())
                {
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(Users user)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.Users.Update(user);
                try
                {
                    var result = await context.SaveChangesAsync();
                    return result > 0; // return true if update worked
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false; // update failed due to concurrency
                }
                catch (Exception)
                {
                    throw; // bubble up unexpected exceptions
                }
            }
        }


        public async Task DeleteUserAsync(int id)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
                if (user != null)
                {
                    context.Users.Remove(user);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateRefreshToken(int userId, string refreshToken)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var user = await context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays
                    (7);
                await context.SaveChangesAsync();
            }
        }
        // ✅ RevokeRefreshToken
        public async Task RevokeRefreshToken(string username)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user != null)
            {
                user.RefreshToken = null;
                await context.SaveChangesAsync();
            }
        }


    }
}
