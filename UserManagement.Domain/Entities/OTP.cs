using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Domain.Entities
{
    public class OTP
    {
        public int Id { get; set; }
        public User User{ get; set; }
        public int UserId{ get; set; }
        public string Otp { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
