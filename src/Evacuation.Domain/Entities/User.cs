
namespace Evacuation.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!; // เก็บ hash
        public bool IsActive { get; private set; } = true;

        // FK
        public int RoleId { get; private set; }

        protected User() { }

        public User(string username, string email, string passwordHash, int roleId)
        {
            ValidateUser(username, email, passwordHash, roleId);
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            RoleId = roleId;
        }

        public void Update(string? username = null, string? email = null, int? roleId = null)
        {
            if (!string.IsNullOrWhiteSpace(username)) Username = username;
            if (!string.IsNullOrWhiteSpace(email)) Email = email;
            if (roleId.HasValue && roleId.Value > 0) RoleId = roleId.Value;
            SetUpdateAt();
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            SetUpdateAt();
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash)) throw new ArgumentException("Password is required.");
            PasswordHash = newPasswordHash;
            SetUpdateAt();
        }


        private void ValidateUser(string username, string email, string passwordHash, int roleId)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.", nameof(username));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("PasswordHash is required.", nameof(passwordHash));
            if (roleId <= 0)
                throw new ArgumentException("RoleId must be greater than zero.", nameof(roleId));
        }
    }
}
