using Evacuation.Application.DTOs.User;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evacuation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var result = await _userService.GetAllUsersAsync();
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while retrieving users", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // GET api/<UsersController>/id/5
        [HttpGet("id/{id:int}")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while retrieving users", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // GET api/<UsersController>/username/{username}
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUsernameAsync(string username)
        {
            var result = await _userService.GetUserByUsernameAsync(username);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while retrieving users", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

      
        //PUT api/<UsersController>/5/role
        [HttpPut("{id}/newrole")]
        [Authorize(Roles = "ADMIN")] // ให้เฉพาะ Admin เปลี่ยน Role ได้
        public async Task<IActionResult> UpdateUserRole(int id, [FromQuery] RoleType newRole)
        {
            var result = await _userService.UpdateUserRoleAsync(id, newRole);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while updating user role", Error = result.Exception.Message });
                }
                else
                {
                    return BadRequest(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserDto updateDto)
        {
            var result = await _userService.UpdateUserAsync(id, updateDto);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while updating user infomation", Error = result.Exception.Message });
                }
                else
                {
                    return BadRequest(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        //// PUT api/<UsersController>/5
        //[HttpPut("ChangePassword/{id}")]
        //public async Task<IActionResult> ChangePasswordAsync(int id, [FromBody] UpdateUserDto updateDto)
        //{
        //}

        //// PUT api/<UsersController>/5
        //[HttpPut("set-active-status/{id}")]
        //public async Task<IActionResult> SetUserActiveStatusAsync(int id, )
        //{
        //}

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")] // ให้เฉพาะ Admin ลบผู้ใช้ได้
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while deleting user", Error = result.Exception.Message });
                }
                else
                {
                    return BadRequest(new { Message = result.Message });
                }
            }
            return Ok(new { Message = result.Message });
        }
    }
}
