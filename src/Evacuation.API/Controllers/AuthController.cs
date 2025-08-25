using Evacuation.Application.DTOs.User;
using Evacuation.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evacuation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // POST api/<AuthController>/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _userService.LoginAsync(loginDto);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred during login", Error = result.Exception.Message });
                }
                else
                {
                    return Unauthorized(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // POST api/<AuthController>/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var result = await _userService.RegisterUserAsync(registerDto);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred during registration", Error = result.Exception.Message });
                }
                else
                {
                    return BadRequest(new { Message = result.Message, Errors = result.Errors });
                }
            }
            return Ok(result.Data);
        }
    }
}
