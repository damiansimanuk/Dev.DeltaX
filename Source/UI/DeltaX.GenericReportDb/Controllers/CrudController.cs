using DeltaX.GenericReportDb.Controllers.Models;
using DeltaX.GenericReportDb.Dto;
using DeltaX.GenericReportDb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeltaX.GenericReportDb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CrudController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly CrudServicePool crudServicePool;

        public CrudController(IUserService userService, CrudServicePool crudServicePool)
        {
            this.userService = userService;
            this.crudServicePool = crudServicePool;
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

        private bool HasPermission(CrudService service, int create = 0, int read = 0, int update = 0, int delete = 0)
        {
            var user = GetCurrentUserAsync().Result;
            return user.HasPermission(service.Configuration.PermissionsRoles, create, read, update, delete);
        }

        [AllowAnonymous]
        [HttpGet("_config/{configName}")]
        public IActionResult GetConfiguration(string configName)
        {
            var service = crudServicePool.GetService(configName);
            if (service == null)
                return BadRequest(new { message = "configName not found" });

            return Ok(service.Configuration);
        }

        [HttpGet("_all_config_name")]
        public IActionResult GetConfigurations()
        { 
            var user = GetCurrentUserAsync().Result;

            var result = crudServicePool.GetAllServices()
                .Select(s => s.Configuration)
                .Where(c => user.HasPermission(c.PermissionsRoles, read: 1))
                .Select(c => new { c.Name, c.DisplayName});

            return Ok(result);
        }

         
        [HttpGet("_search/{configName}/{prefix}")]
        public IActionResult SearchList(string configName, string prefix, [FromQuery] string q)
        {
            var service = crudServicePool.GetService(configName);
            if (service == null || !service.ContainPrefix(EndpointFunction.SearchList, prefix))
            {
                return BadRequest(new { message = "Endpoint not found" });
            }

            if (!HasPermission(service, read: 1))
            {
                return Forbid();
            }

            var js = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(q);

            return Ok(service.SearchList(prefix, js));
        }


        [HttpPost("_search/{configName}/{prefix}")]
        public IActionResult SearchList(string configName, string prefix, [FromBody] Dictionary<string, JsonElement> data)
        {
            var service = crudServicePool.GetService(configName);
            if (service == null || !service.ContainPrefix(EndpointFunction.SearchList, prefix))
            {
                return BadRequest(new { message = "Endpoint not found" });
            }

            if (!HasPermission(service, read: 1))
            {
                return Forbid();
            }

            return Ok(service.SearchList(prefix, data));
        }


        [HttpGet("{configName}/{prefix}")]
        public IActionResult GetList(
            string configName,
            string prefix,
            [FromQuery] int perPage = 10, [FromQuery] int page = 1
            )
        {
            var service = crudServicePool.GetService(configName);
            if (service == null || !service.ContainPrefix(EndpointFunction.GetList, prefix))
            {
                return BadRequest(new { message = "Endpoint not found" });
            }

            if (!HasPermission(service, read: 1))
            {
                return Forbid();
            }

            return Ok(service.GetList(prefix, perPage, page));
        }


        [HttpPost("{configName}/{prefix}")]
        public IActionResult InsertList(string configName, string prefix, [FromBody] Dictionary<string, JsonElement> data)
        {
            var service = crudServicePool.GetService(configName);
            if (service == null || !service.ContainPrefix(EndpointFunction.InsertList, prefix))
            {
                return BadRequest(new { message = "Endpoint not found" });
            }
            if (!HasPermission(service, create: 1))
            {
                return Forbid();
            }

            return Created("", new { result = service.InsertList(prefix, data) });
        }

        [HttpGet("{configName}/{prefix}/{primaryKeys}")]
        public IActionResult GetItem(string configName, string prefix, string primaryKeys)
        {
            var service = crudServicePool.GetService(configName);
            if (service == null || !service.ContainPrefix(EndpointFunction.GetItem, prefix))
            {
                return BadRequest(new { message = "Endpoint not found" });
            }
            if (!HasPermission(service, read: 1))
            {
                return Forbid();
            }

            return Ok(service.GetItem(prefix, primaryKeys));
        }

        [HttpPut("{configName}/{prefix}/{primaryKeys}")]
        public async Task<IActionResult> UpdateItemAsync(
            string configName,
            string prefix,
            string primaryKeys,
            [FromBody] Dictionary<string, JsonElement> data
            )
        {
            var service = crudServicePool.GetService(configName);
            if (service == null || !service.ContainPrefix(EndpointFunction.UpdateItem, prefix))
            {
                return BadRequest(new { message = "Endpoint not found" });
            }
            if (!HasPermission(service, update: 1))
            {
                return Forbid();
            }

            var result = await service.UpdateItemAsync(prefix, primaryKeys, data); 
            return Ok(new { result });
        }


        [HttpDelete("{configName}/{prefix}/{primaryKeys}")]
        public IActionResult DeleteItem(string configName, string prefix, string primaryKeys)
        {
            var service = crudServicePool.GetService(configName);
            if (service == null || !service.ContainPrefix(EndpointFunction.DeleteItem, prefix))
            {
                return BadRequest(new { message = "Endpoint not found" });
            }
            if (!HasPermission(service, delete: 1))
            {
                return Forbid();
            }

            return Ok(new { result = service.DeleteItem(prefix, primaryKeys) });
        }

        [HttpPut("files/{configName}/{prefix}/{primaryKeys}/{fieldName}")]
        public IActionResult UploadFile(string configName, string prefix, string primaryKeys, string fieldName, IFormFile file)
        {
            var service = crudServicePool.GetService(configName);
            if (service == null || !service.ContainPrefix(EndpointFunction.UploadFile, prefix))
            {
                return BadRequest(new { message = "Endpoint not found" });
            }
            if (!HasPermission(service, create: 1))
            {
                return Forbid();
            }

            byte[] fileContent = new byte[file.Length];
            using (var stream = file.OpenReadStream())
            {
                stream.Read(fileContent, 0, (int)file.Length);
            }

            var fileModel = new FileModel
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileContents = fileContent
            };

            var result = service.UploadFile(prefix, primaryKeys, fieldName, fileModel); 
            return Ok(new { result }); 
        }

        [AllowAnonymous]
        [HttpGet("files/{configName}/{prefix}/{primaryKeys}/{fieldName}")]
        public IActionResult DownloadFile(string configName, string prefix, string primaryKeys, string fieldName, [FromQuery] string fileName = null)
        {
            var service = crudServicePool.GetService(configName);
            if (service == null || !service.ContainPrefix(EndpointFunction.DownloadFile, prefix))
            {
                return BadRequest(new { message = "Endpoint not found" });
            } 
            if (!HasPermission(service, read: 1))
            {
                return Forbid();
            }

            var result = service.DownloadFile(prefix, primaryKeys, fieldName, fileName);
            return File(result.FileContents, result.ContentType, result.FileName);
        }
    }
}
