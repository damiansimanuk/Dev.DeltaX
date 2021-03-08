

namespace DeltaX.GenericReportDb.Repository
{
    using Dapper;
    using DeltaX.GenericReportDb.Dto;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Data;

    public class UserRepository
    {
        IDbConnection connection;

        public UserRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public int CreateRoleIfNotExist(string roleName)
        {
            return connection.Execute(SqlQueries.InsertRole, new { roleName });
        }

        public int CreateAdministrator()
        {
            return connection.Execute(SqlQueries.InsertAdministrator);
        }

        public async Task<User> GetUserAsync(int id)
        {
            var user = await connection.QueryFirstAsync<User>(SqlQueries.GetUserDetail, new { Id = id });
            if (user != null)
            {
                var roles = await connection.QueryAsync<RolePermissions>(SqlQueries.GetRolesByUserId, new { user.Id });
                user.Roles = roles.ToArray();
            }
            return user;

        }

        public async Task<User> GetUserAsync(string username)
        {
            var user = await connection.QueryFirstAsync<User>(SqlQueries.GetUserDetailByName, new { Username = username });
            if (user != null)
            {
                var roles = await connection.QueryAsync<RolePermissions>(SqlQueries.GetRolesByUserId, new { user.Id });
                user.Roles = roles.ToArray();
            }
            return user;
        }

        public Task<IEnumerable<User>> GetUsersAsync()
        {
            return connection.QueryAsync<User>(SqlQueries.GetUserList);
        }

        public Task<int> UpdateAsync(User user)
        {
            return connection.ExecuteAsync(SqlQueries.UpdateUser,
                new { user.Id, user.FullName, user.Email, user.PasswordHash, user.Image });
        }
    }
}
