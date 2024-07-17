using UserManagement.Domain.Entities;
using UserManagement.Domain.Repository;
using UserManagement.DTOs;

namespace UserManagement.Service
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        public UserService(IGenericRepository<User> userRepository)
        {
            this._userRepository = userRepository;
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
                user.Password = userDTO.Password;
                user.Email = userDTO.Email;

                await _userRepository.InsertAsync(user);
                await _userRepository.SaveAsync();
               
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

                existingUser.UserName = userDto.UserName;
                existingUser.Password = userDto.Password;
                existingUser.Email = userDto.Email;

                await _userRepository.Update(existingUser);
                await _userRepository.SaveAsync();

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
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
