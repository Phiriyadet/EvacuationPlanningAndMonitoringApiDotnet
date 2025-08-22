namespace Evacuation.Application.DTOs.Role
{
    public class RoleDto
    {
        public int Id { get; set; }          // รหัส Role
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
    }
}
