using UserManagement.Common;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repository;
using UserManagement.DTOs;
using UserManagement.Infrastructure.Enums;
using UserManagement.Interface;

namespace UserManagement.Service
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly ILogService _logService;
        private readonly IEmailService _emailService;

        public UserService(IGenericRepository<User> userRepository, ILogService logService, IEmailService emailService)
        {
            this._userRepository = userRepository;
            this._logService = logService;
            this._emailService = emailService;
        }

        public async Task<IEnumerable<User>> GetAll()
             => await _userRepository.GetAllAsync();

        public async Task<User> Get(int id)
              => await _userRepository.GetByIdAsync(id);

        public async Task Add(UserDTO userDTO)
        {
            try
            {
                User user = new();
                user.UserName = userDTO.UserName;
                user.Password = EncryptDecrypt.Encrypt(userDTO.Password);
                user.Email = userDTO.Email;
                var emailExist = _userRepository.FirstOrDefault(x => x.Email == user.Email);
                if (emailExist != null)
                    throw new Exception("Please try another email, this email is not available.");

                await _userRepository.InsertAsync(user);
                await _userRepository.SaveAsync();
                await _logService.TransactionLog(TransactionType.Success, "User Added Successfully!", "Admin");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task Edit(UserDTO userDto)
        {
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(userDto.Id);
                if (existingUser == null)
                    throw new KeyNotFoundException($"User with ID {userDto.Id} was not found.");
                if (existingUser.Email != userDto.Email)
                {
                    var emailExist = _userRepository.FirstOrDefault(x => x.Email == userDto.Email);
                    if (emailExist != null)
                        throw new Exception("Please try another email, this email is not available.");

                }
                existingUser.UserName = userDto.UserName;
                existingUser.Password = EncryptDecrypt.Encrypt(userDto.Password);
                existingUser.Email = userDto.Email;

                await _userRepository.Update(existingUser);
                await _userRepository.SaveAsync();
                await _logService.TransactionLog(TransactionType.Success, $"User '{existingUser.UserName}'  Updated Successfully ", "Admin");


            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task Delete(int id)
        {
            try
            {
                User user = await this.Get(id);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {id} was not found.");

                _userRepository.Delete(user);
                await _userRepository.SaveAsync();
                await _logService.TransactionLog(TransactionType.Success, $"User '{user.UserName}' Deleted Successfully ", "Admin");

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task UpdateVerificationStatus(int id, bool isVerified)
        {
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                    throw new KeyNotFoundException($"User with ID {id} was not found.");

                existingUser.IsEmailVerified = true;

                await _userRepository.Update(existingUser);
                await _userRepository.SaveAsync();
                await _logService.TransactionLog(TransactionType.Success, $"Email verification status Updated Successfully for User {existingUser.UserName}", "Admin");


            }
            catch (Exception)
            {
                throw;
            }


        }
        public User GetByEmail(string email)
        {
            try
            {
                var user = _userRepository.FirstOrDefault(x => x.Email == email);
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task SendResetPasswordEmail(int userId)
        {
            try
            {

                var user = await this.Get(userId);
                if (user == null) throw new Exception("User not found.");

                _emailService.SendResetPasswordEmail(user.Email, user.UserName, "");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
