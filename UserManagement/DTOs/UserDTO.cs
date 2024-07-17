using System.ComponentModel.DataAnnotations;

namespace UserManagement.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long and must be less than 18 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }

}
