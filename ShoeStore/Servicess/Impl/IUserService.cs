using ShoeStore.Model.Entity;

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


