﻿using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Models;

namespace UserManagement.Infrastructure.Authorization
{
    public class JwtAuthorization : IJwtAuthorization
    {
        private readonly IJwtSetting _jwtSetting;

        public JwtAuthorization(IOptions<JwtSettings> jwtSetting)
        {
            _jwtSetting = jwtSetting.Value;
        }


        public string CreateToken(string username)
        {
            var jwtSetting = _jwtSetting;
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes(jwtSetting.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                 new Claim(ClaimTypes.Name, username)
             }),
                Expires = DateTime.UtcNow.AddMinutes(jwtSetting.ExpirationMinutes),
                Issuer = jwtSetting.Issuer,
                Audience = jwtSetting.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public IEnumerable<Claim> ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSetting.SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = _jwtSetting.Audience,
                    ValidIssuer = _jwtSetting.Issuer,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken.Claims;
            }
            catch
            {
                return Enumerable.Empty<Claim>();
            }
        }
    }
}

