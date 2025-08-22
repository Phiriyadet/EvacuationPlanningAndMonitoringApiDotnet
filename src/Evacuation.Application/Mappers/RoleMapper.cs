using Evacuation.Application.DTOs.Role;
using Evacuation.Domain.Entities;


namespace Evacuation.Application.Mappers
{
    public static class RoleMapper
    {
        public static RoleDto ToDto(this Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
            };
        }

        public static Role CreateToEntity(this CreateRoleDto createDto)
        {
            return new Role(createDto.Name,createDto.Description);
        }

        public static Role UpdateToEntity(this UpdateRoleDto updateDto, Role existingRole)
        {
            // Use null-coalescing operator to provide default values if properties are null
            existingRole.Update(
                updateDto.Name ?? string.Empty,
                updateDto.Description ?? string.Empty
            );
            return existingRole;
        }
    }
}
