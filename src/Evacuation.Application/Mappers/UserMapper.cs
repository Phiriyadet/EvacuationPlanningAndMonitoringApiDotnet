using Evacuation.Application.DTOs.User;
using Evacuation.Domain.Entities;
using Evacuation.Domain.Enums;

namespace Evacuation.Application.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActice = user.IsActive,
                RoleName = user.Role.ToString().ToUpper(),
            };
        }

        public static User CreateToEntity(this RegisterUserDto createDto, string passwordHash, RoleType role)
        {
            return new User
            (
                createDto.Username,
                createDto.Email,
                passwordHash,
                role
            );
        }

        public static User UpdateToEntity(this UpdateUserDto updateDto, User existingUser)
        {
            existingUser.Update
            (
                updateDto.Username,
                updateDto.Email
            );
            return existingUser;
        }
    }
}
