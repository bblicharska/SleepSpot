using IdentityService.Application.Dto;
using IdentityService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using Shared.Exceptions;
using System.Security.Claims;

namespace IdentityAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get() 
        {
            var result = await _userService.GetAllAsync(); 
            return Ok(result);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete(Guid userId)
        {
            await _userService.DeleteAsync(userId);
            return NoContent();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto) 
        {
            try
            {
                var tokenDto = await _userService.RegisterAsync(registerUserDto); 
                return Ok(tokenDto); 
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                if (loginUserDto == null || string.IsNullOrWhiteSpace(loginUserDto.Email) || string.IsNullOrWhiteSpace(loginUserDto.Password))
                {
                    return BadRequest("Invalid login data.");
                }

                var tokenDto = await _userService.LoginAsync(loginUserDto); 
                return Ok(tokenDto);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Something went wrong");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId) 
        {
            try
            {
                var userDto = await _userService.GetUserByIdAsync(userId);
                return Ok(userDto); 
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message); 
            }
        }

        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var userDto = await _userService.GetUserByEmailAsync(email);
                return Ok(userDto);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUserDto) 
        {
            try
            {
                await _userService.UpdateUserAsync(userId, updateUserDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{userId}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                await _userService.ChangePasswordAsync(userId, changePasswordDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("validate-token")]
        [HttpGet]
        public IActionResult ValidateToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("Invalid token");
            }

            return Ok(new { Message = "Token is valid" });
        }
    }
}
