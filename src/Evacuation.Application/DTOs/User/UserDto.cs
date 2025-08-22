namespace Evacuation.Application.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsActice { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
