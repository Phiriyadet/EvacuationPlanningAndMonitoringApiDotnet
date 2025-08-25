using Evacuation.Application.DTOs.User;
using Evacuation.Domain.Enums;
using Evacuation.Shared.Result;

namespace Evacuation.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<OperationResult<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<OperationResult<UserDto>> GetUserByIdAsync(int userId);
        Task<OperationResult<UserDto>> GetUserByUsernameAsync(string username);
        Task<OperationResult<IEnumerable<UserDto>>> GetAllUsersAsync();
        Task<OperationResult<UserDto>> RegisterUserAsync(RegisterUserDto createDto);
        Task<OperationResult<UserDto>> UpdateUserAsync(int userId, UpdateUserDto updateDto);
        Task<OperationResult<UserDto>> UpdateUserRoleAsync(int userId, RoleType newRole);
        Task<OperationResult<string>> ChangePasswordAsync(int userId, ChangePasswordDto changePassDto);
        Task<OperationResult<string>> SetUserActiveStatusAsync(int userId, bool isActive);
        Task<OperationResult<bool>> DeleteUserAsync(int userId);
    }
}
