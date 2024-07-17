using System.Security.Claims;

namespace UserManagement.Infrastructure.Authorization
{
    public interface IJwtAuthorization
    {
        string CreateToken(string username);
        IEnumerable<Claim> ValidateToken(string token);
    }
}