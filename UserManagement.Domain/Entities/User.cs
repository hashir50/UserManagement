using System.ComponentModel.DataAnnotations;

namespace UserManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string UserName { get; set; }
        public string Password { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}
