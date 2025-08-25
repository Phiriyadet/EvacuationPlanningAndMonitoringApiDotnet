using System.ComponentModel.DataAnnotations;

namespace Evacuation.Application.DTOs.User
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;
        [Required]
        public string NewPassword { get; set; } = null!;
        [Required]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
