using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.Service;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _userService.GetAll();
                return Ok(response);
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
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _userService.Get(id);
                return response == null ? NotFound(new { message = $"User with ID {id} was not found." }) : Ok(response);
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDTO user)
        {
            try
            {
                await _userService.Add(user);
                return Created();
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

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserDTO user)
        {
            try
            {
                var userById = await _userService.Get(user.Id);
                if (userById is null)
                    return NotFound(new { message = $"User with ID {user.Id} was not found." });

                await _userService.Edit(user);
                return Ok();

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.Delete(id);
                return NoContent();
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
