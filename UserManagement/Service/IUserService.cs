using UserManagement.Domain.Entities;
using UserManagement.DTOs;

namespace UserManagement.Service
{
    public interface IUserService
    {
        Task Add(UserDTO userDTO);
        Task Delete(int id);
        Task Edit(UserDTO userDto);
        Task<User> Get(int id);
        Task<IEnumerable<User>> GetAll();
    }
}