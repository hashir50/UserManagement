using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Models;
using UserManagement.Infrastructure.Enums;
using UserManagement.Interface;
using UserManagement.Service;

namespace UserManagement.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogService _logService;
        public AuthController(IAuthService authService, ILogService logService)
        {
            _authService = authService;
            _logService = logService;
        }
        [HttpPost]
        public IActionResult Authenticate([FromBody] Auth oAuth)
        {
            try
            {
                var token = _authService.Authenticate(oAuth);
                if (token == null)
                    return Unauthorized(new { message = "Invalid Credentials" });

                 _logService.TransactionLog(TransactionType.Success, $"User Authenticated Successfully!","Admin").Wait();

                return Ok(new
                {
                    AccessToken = token,
                    AccessType = "Bearer",
                });

            }
            catch (Exception ex)
            {
                var errorDetails = new
                {
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                };
                _logService.TransactionLog(TransactionType.Error, $"message ={ex.Message},innerException = {ex.InnerException?.Message}", "Admin").Wait();
                return BadRequest(errorDetails);
            }
        }

    }

}
