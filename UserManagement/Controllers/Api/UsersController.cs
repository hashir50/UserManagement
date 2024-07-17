using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using UserManagement.Domain.Entities;
using UserManagement.DTOs;
using UserManagement.Infrastructure.Enums;
using UserManagement.Interface;
using UserManagement.Service;

namespace UserManagement.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOtpService _otpService;
        private readonly ILogService _logService;
        public UsersController(IUserService userService, IOtpService otpService, ILogService logService)
        {
            _userService = userService;
            _otpService = otpService;
            _logService = logService;
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
                await _logService.TransactionLog(TransactionType.Error, $"message ={ex.Message},innerException = {ex.InnerException?.Message}", "Admin");
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
                await _logService.TransactionLog(TransactionType.Error, $"message ={ex.Message},innerException = {ex.InnerException?.Message}", "Admin");
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
                await _logService.TransactionLog(TransactionType.Error, $"message ={ex.Message},innerException = {ex.InnerException?.Message}", "Admin");
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
                await _logService.TransactionLog(TransactionType.Error, $"message ={ex.Message},innerException = {ex.InnerException?.Message}", "Admin");
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
                await _logService.TransactionLog(TransactionType.Error, $"message ={ex.Message},innerException = {ex.InnerException?.Message}", "Admin");
                return BadRequest(errorDetails);
            }
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(int id)
        {
            try
            {
                await _otpService.Send(id);
                return Ok("Otp Sent SuccessFully!");
            }
            catch (Exception ex)
            {
                var errorDetails = new
                {
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                };
                await _logService.TransactionLog(TransactionType.Error, $"message ={ex.Message},innerException = {ex.InnerException?.Message}", "Admin");
                return BadRequest(errorDetails);
            }
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(int userId, string otp)
        {
            try
            {
                var user = await _userService.Get(userId);
                if (user == null)
                {
                    await _logService.TransactionLog(TransactionType.Success, $"User with ID {userId} was not found.", "Admin");
                    return NotFound(new { message = $"User with ID {userId} was not found." });
                }

                bool isVerified = await _otpService.verify(userId,otp);
                if (!isVerified)
                {
                    await _logService.TransactionLog(TransactionType.Success, $"Invalid Or Expired OTP", user.UserName);
                    return Unauthorized("Invalid Or Expired OTP");
                }

                await _userService.UpdateVerificationStatus(userId,true);
                await _logService.TransactionLog(TransactionType.Success, $"User '{user.UserName}'  Updated Successfully ", "Admin");

                return Ok("Email Verified!");
            }
            catch (Exception ex)
            {
                var errorDetails = new
                {
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                };
                await _logService.TransactionLog(TransactionType.Error, $"message ={ ex.Message},innerException = {ex.InnerException?.Message}", "Admin");

                return BadRequest(errorDetails);
            }
        }
    }
}
