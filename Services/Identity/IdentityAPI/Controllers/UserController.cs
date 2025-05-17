using IdentityService.Application.Dto;
using IdentityService.Application.Services;
using IdentityService.Domain.Exceptions;
using IdentityService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<IEnumerable<UserDto>>> Get() // Zmieniamy metodę na asynchroniczną
        {
            var result = await _userService.GetAllAsync(); // Czekamy na wynik operacji
            return Ok(result);
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id) // Zmieniamy metodę na asynchroniczną
        {
            await _userService.DeleteAsync(id); // Czekamy na zakończenie operacji
            return NoContent();
        }

        // Rejestracja nowego użytkownika
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto) // Asynchroniczna metoda
        {
            try
            {
                var tokenDto = await _userService.RegisterAsync(registerUserDto); // Czekamy na wynik
                return Ok(tokenDto); // Zwracamy token w odpowiedzi
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Logowanie użytkownika
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                if (loginUserDto == null || string.IsNullOrWhiteSpace(loginUserDto.UsernameOrEmail) || string.IsNullOrWhiteSpace(loginUserDto.Password))
                {
                    return BadRequest("Invalid login data.");
                }

                var tokenDto = await _userService.LoginAsync(loginUserDto); // Zmieniamy na await
                return Ok(tokenDto); // Zwracamy token w odpowiedzi
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message); // Zwracamy błąd, jeśli dane są niepoprawne
            }
            catch (Exception ex)
            {
                // Logowanie błędów ogólnych
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Something went wrong");
            }
        }

        // Pobieranie danych użytkownika po ID
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid userId) // Asynchroniczna metoda
        {
            try
            {
                var userDto = await _userService.GetUserByIdAsync(userId); // Czekamy na wynik
                return Ok(userDto); // Zwracamy dane użytkownika
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message); // Zwracamy 404, jeśli użytkownik nie istnieje
            }
        }

        // Aktualizowanie danych użytkownika
        [HttpPut("{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUserDto) // Asynchroniczna metoda
        {
            try
            {
                await _userService.UpdateUserAsync(userId, updateUserDto); // Czekamy na zakończenie operacji
                return NoContent(); // Zwracamy 204 po udanej aktualizacji
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Zmiana hasła użytkownika
        [HttpPut("{userId}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] ChangePasswordDto changePasswordDto) // Asynchroniczna metoda
        {
            try
            {
                await _userService.ChangePasswordAsync(userId, changePasswordDto); // Czekamy na zakończenie operacji
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
        [Authorize]
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
