using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Models;
using UserManagement.Service;

namespace UserManagement.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost]
        public IActionResult Authenticate([FromBody] Auth oAuth)
        {
            try
            {
                var token = _authService.Authenticate(oAuth);
                if (token == null)
                    return Unauthorized(new { message = "Invalid Credentials" });

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
                return BadRequest(errorDetails);
            }
        }

    }

}
