
using Evacuation.Domain.Enums;

namespace Evacuation.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!; // เก็บ hash
        public bool IsActive { get; private set; } = true;
        public RoleType Role { get; private set; }

        protected User() { }

        public User(string username, string email, string passwordHash, RoleType role)
        {
            ValidateUser(username, email, passwordHash, role);
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }

        public void Update(string? username = null, string? email = null)
        {
            if (!string.IsNullOrWhiteSpace(username)) Username = username;
            if (!string.IsNullOrWhiteSpace(email)) Email = email;
            SetUpdateAt();
        }

        public void AssignRole(RoleType role)
        {
            if (!Enum.IsDefined(typeof(RoleType), role)) 
                throw new ArgumentException("Invalid role.", nameof(role));
            Role = role;
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


        private void ValidateUser(string username, string email, string passwordHash, RoleType role)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.", nameof(username));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("PasswordHash is required.", nameof(passwordHash));
            if (!Enum.IsDefined(typeof(RoleType), role))
                throw new ArgumentException("Invalid role.", nameof(role));
        }
    }
}
