using IdentityService.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Services
{
    public interface IUserService
    {
        Task<TokenDto> RegisterAsync(RegisterUserDto registerUserDto);
        Task<TokenDto> LoginAsync(LoginUserDto loginUserDto);
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<List<UserDto>> GetAllAsync();
        Task DeleteAsync(Guid id);
        Task UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
        Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
    }

}
