using System.ComponentModel.DataAnnotations;

namespace Evacuation.Application.DTOs.Role
{
    public class CreateRoleDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
    }
}
