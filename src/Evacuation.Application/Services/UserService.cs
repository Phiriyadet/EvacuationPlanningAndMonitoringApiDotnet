using Evacuation.Application.DTOs.User;
using Evacuation.Application.Mappers;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Domain.Entities;
using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Evacuation.Shared.Result;
using Evacuation.Shared.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Evacuation.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserService> _logger;
        public UserService(IConfiguration config, IUserRepository userRepo, ILogger<UserService> logger)
        {
            _config = config;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<OperationResult<string>> ChangePasswordAsync(int userId, ChangePasswordDto changePassDto)
        {
            _logger.LogInformation("At Time {Time}, ChangePasswordAsync called", DateTime.UtcNow);
            try 
            {
                var errors = ValidationHelper.ValidateObject(changePassDto);
                if (errors.Any())
                {
                    return OperationResult<string>.Fail(errors, "Validation errors occurred");
                }
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    return OperationResult<string>.Fail("User not found", null);
                }
                if (!BCrypt.Net.BCrypt.Verify(changePassDto.CurrentPassword, user.PasswordHash))
                {
                    return OperationResult<string>.Fail("Old password is incorrect", null);
                }
                user.ChangePassword(BCrypt.Net.BCrypt.HashPassword(changePassDto.NewPassword));
                await _userRepo.UpdateAsync(user.Id, user);
                return OperationResult<string>.Ok("Password changed successfully", "Password changed successfully");
            }
            catch (DbUpdateException dbEx)
            {
                return OperationResult<string>.Fail("Database update error occurred while changing password", null, dbEx);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail("Error changing password", null, ex);
            }
        }

        public async Task<OperationResult<UserDto>> RegisterUserAsync(RegisterUserDto registerDto)
        {
            _logger.LogInformation("At Time {Time}, RegisterUserAsync called", DateTime.UtcNow);
            try
            {
                var errors = ValidationHelper.ValidateObject(registerDto);
                if (errors.Any())
                {
                    _logger.LogWarning("Validation errors occurred while registering user: {Errors}", string.Join(", ", errors));
                    return OperationResult<UserDto>.Fail(errors, "Validation errors occurred");
                }

                var existingUser = await _userRepo.GetByUsernameAsync(registerDto.Username);
                if (existingUser != null)
                {
                    _logger.LogWarning("Username {Username} already exists", registerDto.Username);
                    return OperationResult<UserDto>.Fail("Username already exists", null);
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                var user = registerDto.CreateToEntity(passwordHash, RoleType.User);
                await _userRepo.AddAsync(user);

                _logger.LogInformation("User with ID {UserId} created successfully", user.Id);
                return OperationResult<UserDto>.Ok(user.ToDto(), "User created successfully");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error occurred while creating user");
                return OperationResult<UserDto>.Fail("Database update error occurred while creating the user", null, dbEx);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Error generating user ID");
                return OperationResult<UserDto>.Fail($"Error generating user ID: {argEx.Message}", null, argEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating user");
                return OperationResult<UserDto>.Fail("Error creating user", null, ex);
            }
        }

        public async Task<OperationResult<bool>> DeleteUserAsync(int userId)
        {
            _logger.LogWarning("At Time {Time}, DeleteUserAsync called for User ID {UserId}", DateTime.UtcNow, userId);
            try
            {
                var deleted = await _userRepo.DeleteAsync(userId);
                if (deleted)
                {
                    return OperationResult<bool>.Ok(true, $"User with ID {userId} deleted successfully");
                }
                else
                {
                    return OperationResult<bool>.Fail("User not found", false);
                }
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail("Error deleting user", false, ex);
            }
        }

        public async Task<OperationResult<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            _logger.LogInformation("At Time {Time}, GetAllUsersAsync called", DateTime.UtcNow);
            try
            {
                var users = await _userRepo.GetAllAsync();
                if (users == null || !users.Any())
                {
                    return OperationResult<IEnumerable<UserDto>>.Fail("No users found", null);
                }
                var userDtos = users.Select(u => u.ToDto());
                return OperationResult<IEnumerable<UserDto>>.Ok(userDtos, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<UserDto>>.Fail("Error retrieving users", null, ex);
            }
        }

        public async Task<OperationResult<UserDto>> GetUserByIdAsync(int userId)
        {
            _logger.LogInformation("At Time {Time}, GetUserByIdAsync called for User ID {UserId}", DateTime.UtcNow, userId);
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    return OperationResult<UserDto>.Fail("User not found", null);
                }
                return OperationResult<UserDto>.Ok(user.ToDto(), "User retrieved successfully");
            }
            catch (Exception ex)
            {
                return OperationResult<UserDto>.Fail("Error retrieving user", null, ex);
            }
        }

        public async Task<OperationResult<UserDto>> GetUserByUsernameAsync(string username)
        {
            _logger.LogInformation("At Time {Time}, GetUserByUsernameAsync called for Usernae {Username}", DateTime.UtcNow, username);
            try
            {
                var user = await _userRepo.GetByUsernameAsync(username);
                if (user == null)
                {
                    return OperationResult<UserDto>.Fail("User not found", null);
                }
                return OperationResult<UserDto>.Ok(user.ToDto(), "User retrieved successfully");
            }
            catch (Exception ex)
            {
                return OperationResult<UserDto>.Fail("Error retrieving user", null, ex);
            }
        }

        #region Login
        public async Task<OperationResult<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var errors = ValidationHelper.ValidateObject(loginDto);
                if (errors.Any())
                {
                    return OperationResult<AuthResponseDto>.Fail(errors, "Validation errors occurred");
                }

                var user = await _userRepo.GetByUsernameAsync(loginDto.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return OperationResult<AuthResponseDto>.Fail("Invalid username or password", null);
                }

                var token = GenerateToken(user);
                var authResponse = new AuthResponseDto
                {
                    Username = user.Username,
                    Email = user.Email,
                    Token = token,
                };
                return OperationResult<AuthResponseDto>.Ok(authResponse, "Login successful");
            }
            catch (Exception ex)
            {
                return OperationResult<AuthResponseDto>.Fail("Error during login", null, ex);
            }

        }

        private string GenerateToken(User user)
        {
            // 1) สร้าง Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString().ToUpper())
            };

            // 2) Key สำหรับเซ็น token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3) สร้าง token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1), // หมดอายุใน 1 วัน
                signingCredentials: creds
            );

            // 4) return เป็น string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        public async Task<OperationResult<string>> SetUserActiveStatusAsync(int userId, bool isActive)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<UserDto>> UpdateUserAsync(int userId, UpdateUserDto updateDto)
        {
            try
            {
                var errors = ValidationHelper.ValidateObject(updateDto);
                if (errors.Any())
                {
                    return OperationResult<UserDto>.Fail(errors, "Validation errors occurred");
                }

                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    return OperationResult<UserDto>.Fail("User not found", null);
                }

                user.Update(updateDto.Username, updateDto.Email);
                await _userRepo.UpdateAsync(user.Id, user);
                return OperationResult<UserDto>.Ok(user.ToDto(), "User updated successfully");
            }
            catch (DbUpdateException dbEx)
            {
                return OperationResult<UserDto>.Fail("Database update error occurred while updating user", null, dbEx);
            }
            catch (Exception ex)
            {
                return OperationResult<UserDto>.Fail("Error updating user", null, ex);
            }
        }

        public async Task<OperationResult<UserDto>> UpdateUserRoleAsync(int userId, RoleType newRole)
        {
            try
            {
                var errors = ValidationHelper.ValidateObject(newRole);
                if (errors.Any())
                {
                    return OperationResult<UserDto>.Fail(errors, "Validation errors occurred");
                }

                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    return OperationResult<UserDto>.Fail("User not found", null);
                }

                user.AssignRole(newRole);
                await _userRepo.UpdateAsync(user.Id, user);
                return OperationResult<UserDto>.Ok(user.ToDto(), "User role updated successfully");
            }
            catch (DbUpdateException dbEx)
            {
                return OperationResult<UserDto>.Fail("Database update error occurred while updating user role", null, dbEx);
            }
            catch (Exception ex)
            {
                return OperationResult<UserDto>.Fail("Error updating user role", null, ex);
            }
        }

    }
}
