﻿using UserManagement.Interface;

namespace UserManagement.StartUp.Middleware
{
    public class JwtMiddleware 
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthService authService)
        {
            if (IsApiEndpoint(context))
            {
                var jwtToken = context.Request.Headers["Authorization"].FirstOrDefault()?
                                                                       .Split(" ")
                                                                       .LastOrDefault();
                if (jwtToken != null)
                {
                    var jwtClaims = authService.ValidateToken(jwtToken);
                    if (jwtClaims.Count() > 0)
                        context.Items["JwtUser"] = jwtClaims;
                    else
                        context.Items["JwtUser"] = null;
                }
            }

            await _next(context);
        }
        protected bool IsApiEndpoint(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/api");
        }

    }
}
