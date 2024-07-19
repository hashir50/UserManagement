using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using System.Text;
using UserManagement.Common;
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
        private readonly IEmailService _emailService;

        public UsersController(IUserService userService, IOtpService otpService, ILogService logService, IEmailService emailService)
        {
            _userService = userService;
            _otpService = otpService;
            _logService = logService;
            _emailService = emailService;
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
        public async Task<IActionResult> VerifyEmail(string email)
        {
            try
            {
                var user = _userService.GetByEmail(email);
                if (user is null)
                {
                    await _logService.TransactionLog(TransactionType.Success, $"User with Email '{email}' was not found.", "Admin");
                    return NotFound(new { message = $"User with Email '{email}' was not found." });
                }
                await _otpService.Send(user.Id);
                return Ok(new { message = "Otp Sent SuccessFully!" });
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
                    return Unauthorized(new { message = "Invalid Or Expired OTP" });
                }

                await _userService.UpdateVerificationStatus(userId,true);
                await _logService.TransactionLog(TransactionType.Success, $"User '{user.UserName}'  Updated Successfully ", "Admin");

                return Ok(new { message = "Email Verified!" });
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
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                var user = _userService.GetByEmail(email);
                if (user is null)
                {
                    await _logService.TransactionLog(TransactionType.Success, $"User with Email '{email}' was not found.", "Admin");
                    return NotFound($"User with Email '{email}' was not found.");
                }
                
                if (!user.IsEmailVerified)
                    return Ok(new { message = "The Email is Not Verified !" });
                
                var tokenData = $"{user.Id}::{DateTime.UtcNow.AddMinutes(5):o}";
                var token= EncryptDecrypt.Encrypt(tokenData);
                var callbackUrl = Url.Action(nameof(ResetPassword), "Users", new { token = token }, protocol: HttpContext.Request.Scheme);

                _emailService.SendResetPasswordEmail(user.Email,user.UserName,callbackUrl);

                return Ok(new { message = "If the email is registered, you will receive a password reset link." });

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
        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string token)
        {
            try
            {
                var tokenData = EncryptDecrypt.Decrypt(token);
                var parts = tokenData.Split("::");
                
                if (parts is null || parts?.Length!=2)
                    return BadRequest(new { message = "Invalid Request" });

                if (!DateTime.TryParse(parts[1], out DateTime expiryDate))
                    return BadRequest(new { message = "Invalid Request" });

                if (expiryDate<DateTime.Now)
                    return BadRequest(new { message = "Link Expired" });

                var userId = Convert.ToInt32(parts[0]);
                var user = _userService.Get(userId);

                if (user is null)
                    return BadRequest("Invalid Request.");

                return RedirectToAction("ResetPassword", "Home",  new { token = token });
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
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordDTO model)
        {
            try
            {
                var tokenData = EncryptDecrypt.Decrypt(model.Token);
                var parts = tokenData.Split("::");

                if (parts is null || parts?.Length != 2)
                    return BadRequest(new { message = "Invalid Request" });

                if (!DateTime.TryParse(parts[1], out DateTime expiryDate))
                    return BadRequest(new { message = "Invalid Request" });

                if (expiryDate < DateTime.UtcNow)
                    return BadRequest(new { message = "Link Expired" });

                var userId = Convert.ToInt32(parts[0]);
                var user = await _userService.Get(userId);

                if (user is null)
                    return BadRequest("Invalid Request.");

                user.Password = EncryptDecrypt.Encrypt(model.Password);
                var userDTO = new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = EncryptDecrypt.Encrypt(model.Password),
                };
                await _userService.Edit(userDTO);
                
                return Ok(new { message = "Password has been reset successfully." });
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
    }
}
