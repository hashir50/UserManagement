using UserManagement.Domain.Entities;
using UserManagement.DTOs;

namespace UserManagement.Interface
{
    public interface IUserService
    {
        Task Add(UserDTO userDTO);
        Task Delete(int id);
        Task Edit(UserDTO userDto);
        Task<User> Get(int id);
        Task<IEnumerable<User>> GetAll();
        User GetByEmail(string email);
        Task UpdateVerificationStatus(int id, bool isVerified);
    }
}