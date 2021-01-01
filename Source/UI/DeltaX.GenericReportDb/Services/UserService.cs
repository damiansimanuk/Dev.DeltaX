using DeltaX.GenericReportDb.Dto;
using DeltaX.GenericReportDb.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DeltaX.GenericReportDb.Services
{
    public class UserService : IUserService
    {
        private readonly string secret;
        private readonly PasswordHasher<User> passwordHasher;
        private readonly IConfiguration configuration;
        private readonly UserRepository repository;

        public UserService(IConfiguration configuration, UserRepository userRepository)
        {
            this.configuration = configuration;
            this.repository = userRepository;
            this.secret = configuration.GetValue<string>("UserService:Secret");
            this.passwordHasher = new PasswordHasher<User>();
        }

        public int CreateRoleIfNotExist(string roleName)
        {
            return repository.CreateRoleIfNotExist(roleName);
        }

        public string GenerateToken(User user)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));
            foreach (var r in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, r.RolName));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await GetUserAsync(username);

            if (user == null)
                return null;

            // Create New password
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                await UpdateUserAsync(user, newPassword: password);
                user.Token = GenerateToken(user);
                return user;
            }

            var res = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            switch (res)
            {
                case PasswordVerificationResult.SuccessRehashNeeded:
                    await UpdateUserAsync(user, newPassword: password);
                    user.Token = GenerateToken(user);
                    return user;
                case PasswordVerificationResult.Success:
                    user.Token = GenerateToken(user);
                    return user;
                default:
                    return null;
            }
        }

        public Task<IEnumerable<User>> GetUsersAsync()
        {
            return repository.GetUsersAsync();
        }

        public Task<int> UpdateUserAsync(User user, string newFullName = null, string newEmail = null, string newPassword = null, string newImage = null)
        {
            if (!string.IsNullOrEmpty(newFullName))
                user.FullName = newFullName;
            if (!string.IsNullOrEmpty(newEmail))
                user.Email = newEmail;
            if (!string.IsNullOrEmpty(newPassword))
                user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
            if (!string.IsNullOrEmpty(newImage))
                user.Image = newImage;

            return repository.UpdateAsync(user);
        }

        public Task<User> GetUserAsync(int id)
        {
            return repository.GetUserAsync(id);
        }

        public Task<User> GetUserAsync(string username)
        {
            return repository.GetUserAsync(username);
        }
    }
}
