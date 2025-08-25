using Evacuation.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Evacuation.Application.DTOs.User
{
    public class UpdateUserRoleDto
    {
        [Required]
        [EnumDataType(typeof(RoleType))]
        public RoleType NewRole { get; set; }
    }

}
