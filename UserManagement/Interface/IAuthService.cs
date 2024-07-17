using System.Security.Claims;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Models;

namespace UserManagement.Interface
{
    public interface IAuthService
    {
        string Authenticate(Auth auth);
        IEnumerable<Claim> ValidateToken(string token);
    }
}