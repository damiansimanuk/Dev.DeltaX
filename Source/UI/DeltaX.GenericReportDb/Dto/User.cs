namespace DeltaX.GenericReportDb.Dto
{
    using System;
    using System.Linq;
    using System.Text.Json.Serialization;

    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public RolePermissions[] Roles { get; set; }
        public string Image { get; set; }
        public string Active { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }
        public string Token { get; set; }

        public bool IsAdministrator(string role = "Administrator")
        {
            return Roles.Where(r => r.RolName == role).Count() > 0;
        }

        public bool HasPermission(string[] roles, int create = 0, int read = 0, int update = 0, int delete = 0)
        {
            if (roles.Count() == 0)
                return true;

            return IsAdministrator() ||
               Roles.Where(r =>
               {
                   return roles.Contains(r.RolName)
                       && r.C >= create
                       && r.R >= read
                       && r.U >= update
                       && r.D >= delete;
               }).Count() > 0;
        }
    }
}
