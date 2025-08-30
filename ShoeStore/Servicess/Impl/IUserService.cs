using Microsoft.EntityFrameworkCore.Internal;
using OfficeProject.Authentication;
using ShoeStore.Model.Entity;
using ShoeStore.Shared.Dto;

namespace ShoeStore.Servicess.Impl
{
    public interface IUserService
    {

        Task<Users?> FindUserByUsername(string UserName);
        Task<Users?> FindUserByIdAsync(int id);
        Task AddUserAsync(Users user);
        Task<bool> UpdateUserAsync(Users user);
        Task DeleteUserAsync(int id);
        Task UpdateRefreshToken(int userId, string refreshToken);
        Task RevokeRefreshToken(string username);

    }
}


