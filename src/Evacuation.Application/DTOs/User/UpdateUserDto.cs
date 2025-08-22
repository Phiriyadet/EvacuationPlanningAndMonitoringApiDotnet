using System.ComponentModel.DataAnnotations;

namespace Evacuation.Application.DTOs.User
{
    public class UpdateUserDto
    {
        public string? Username { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
    }
}
