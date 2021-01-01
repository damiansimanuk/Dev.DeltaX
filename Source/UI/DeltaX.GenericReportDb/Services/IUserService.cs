using DeltaX.GenericReportDb.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeltaX.GenericReportDb.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);
        int CreateRoleIfNotExist(string roleName);
        string GenerateToken(User user);
        Task<User> GetUserAsync(int id);
        Task<User> GetUserAsync(string username);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<int> UpdateUserAsync(User user, string newFullName = null, string newEmail = null, string newPassword = null, string newImage = null);
    }
}