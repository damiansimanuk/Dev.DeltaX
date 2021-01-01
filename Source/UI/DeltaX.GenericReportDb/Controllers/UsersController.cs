namespace DeltaX.GenericReportDb.Controllers
{
    using DeltaX.GenericReportDb.Controllers.Models;
    using DeltaX.GenericReportDb.Dto;
    using DeltaX.GenericReportDb.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;


    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        private async Task<User> GetCurrentUserAsync()
        {
            User user = null;
            var userId = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                user = await userService.GetUserAsync(Convert.ToInt32(userId));
            }

            return user
                ?? throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<User> Login(
            [FromBody] AuthenticateModel credential
            )
        {
            var user = await userService.AuthenticateAsync(credential.Username, credential.Password);

            return user
                ?? throw new HttpResponseException(HttpStatusCode.BadRequest);
        }


        [HttpPost("logout")]
        public string Logout()
        {
            // FIXME: Marcar el token en blacklist y sacar de la lista por caducidad.
            return "Ok";
        }

        [HttpPost("refresh-token")]
        public async Task<User> RefreshToken()
        {
            var user = await GetCurrentUserAsync();
            user.Token = userService.GenerateToken(user);
            return user;
        }


        [HttpGet("current")]
        public Task<User> GetUser()
        {
            return GetCurrentUserAsync();
        }

        [HttpPut("{id}")]
        public async Task<User> UpdateUser(int id,
            [FromBody] EditUserModel editUserInfo)
        {
            var currUser = await GetCurrentUserAsync();
            if (currUser.Id != id && !currUser.IsAdministrator())
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            if (!string.IsNullOrEmpty(editUserInfo.Password) && editUserInfo.Password != editUserInfo.ConfirmPassword)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var res = await userService.UpdateUserAsync(currUser, editUserInfo.FullName, editUserInfo.Email,
                editUserInfo.Password, editUserInfo.Image);
            if (res != 1)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return currUser;
        }

        [AllowAnonymous]
        [HttpGet]
        public Task<IEnumerable<User>> GetAll()
        {
            return userService.GetUsersAsync();
        }


        [HttpGet("{id}")]
        public Task<User> GetUser(int id)
        {
            var user = userService.GetUserAsync(id);
            return user
                ?? throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}
