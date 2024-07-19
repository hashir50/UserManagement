using System.Security.Claims;
using UserManagement.Common;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repository;
using UserManagement.Infrastructure.Authorization;
using UserManagement.Interface;

namespace UserManagement.Service
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _authRepository;
        private readonly IJwtAuthorization _jwtAuthorization;


        public AuthService(IJwtAuthorization jwtAuthorization, IGenericRepository<User> authRepository)
        {
            this._authRepository = authRepository;
            this._jwtAuthorization = jwtAuthorization;
        }

        public string Authenticate(Auth auth)
        {
            if (auth == null)
                throw new ArgumentNullException(nameof(auth));
            try
            {
                var user = _authRepository.FirstOrDefault(
                    x => x.Password.Equals(EncryptDecrypt.Encrypt(auth.Password)) && x.Email.Equals(auth.Email));
                if (user is null)
                    return null;
                return this._jwtAuthorization.CreateToken(user.UserName);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error occurred during authentication.", ex);
            }
        }
        public IEnumerable<Claim> ValidateToken(string token)
        {
            return this._jwtAuthorization.ValidateToken(token);
        }

    }
}
