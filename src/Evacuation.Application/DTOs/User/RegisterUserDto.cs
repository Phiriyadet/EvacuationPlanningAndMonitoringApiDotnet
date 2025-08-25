using System.ComponentModel.DataAnnotations;

namespace Evacuation.Application.DTOs.User
{
    public class RegisterUserDto
    {
        [Required] 
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress] 
        public string Email { get; set; } = null!;

        [Required] 
        public string Password { get; set; } = null!;
    }
}
