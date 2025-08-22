using Evacuation.Application.DTOs.User;
using Evacuation.Domain.Entities;

namespace Evacuation.Application.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user, string roleName)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActice = user.IsActive,
                RoleName = roleName
            };
        }

        public static User CreateToEntity(this CreateUserDto createDto, string passwordHash)
        {
            return new User
            (
                createDto.Username,
                createDto.Email,
                passwordHash,
                createDto.RoleId
            );
        }

        public static User UpdateToEntity(this UpdateUserDto updateDto, User existingUser)
        {
            existingUser.Update
            (
                updateDto.Username,
                updateDto.Email,
                updateDto.RoleId
            );
            return existingUser;
        }
    }
}
