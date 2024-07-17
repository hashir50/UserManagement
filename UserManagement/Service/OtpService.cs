using UserManagement.Domain.Entities;
using UserManagement.Domain.Repository;
using UserManagement.Interface;

namespace UserManagement.Service
{
    public class OtpService : IOtpService
    {
        private readonly IGenericRepository<OTP> _otpRepository;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public OtpService(IGenericRepository<OTP> otpRepository, IUserService userService, IEmailService emailService)
        {
            this._otpRepository = otpRepository;
            this._userService = userService;
            this._emailService = emailService;
        }
        public async Task<bool> verify(int userId, string otp)
        {

            try
            {
                var response = _otpRepository.FirstOrDefault(o => o.UserId == userId && o.Otp == otp && o.ExpiryDate > DateTime.UtcNow);
                return response == null ? false : true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task Send(int userId)
        {
            try
            {

                var user = await _userService.Get(userId);
                if (user == null) throw new Exception("User not found.");

                string otp = Generate();
                var otpEntity = new OTP
                {
                    UserId = user.Id,
                    Otp = otp,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(5)
                };

                await _otpRepository.InsertAsync(otpEntity);
                await _otpRepository.SaveAsync();

                _emailService.SendOtpEmail(user.Email,user.UserName, otp);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private string Generate()
        {
            var random = new Random();
            return random.Next(1000, 9999).ToString();
        }
    }
}
