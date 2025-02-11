﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Domain.Models
{
    public interface IJwtSetting
    {
        string Audience { get; set; }
        int ExpirationMinutes { get; set; }
        string Issuer { get; set; }
        int NotBeforeMinutes { get; set; }
        string SecretKey { get; set; }
    }

    public class JwtSettings : IJwtSetting
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int ExpirationMinutes { get; set; }
        public int NotBeforeMinutes { get; set; }
    }
}
