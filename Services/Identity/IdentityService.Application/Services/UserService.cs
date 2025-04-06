using AutoMapper;
using IdentityService.Application.Dto;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Exceptions;
using IdentityService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;

        public UserService(
            IUserUnitOfWork uow,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IMapper mapper)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
        }

        public async Task<TokenDto> RegisterAsync(RegisterUserDto registerUserDto)
        {
            if (registerUserDto == null)
            {
                throw new BadRequestException("User data is null");
            }

            if (await _uow.UserRepository.UserExistsAsync(registerUserDto.Email))
            {
                throw new BadRequestException("A user with this email already exists.");
            }

            if (registerUserDto.Password != registerUserDto.ConfirmPassword)
            {
                throw new BadRequestException("Passwords do not match.");
            }

            var user = new User
            {
                Username = registerUserDto.Username,
                Email = registerUserDto.Email,
                PasswordHash = _passwordHasher.HashPassword(registerUserDto.Password),
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            await _uow.UserRepository.InsertAsync(user);
            await _uow.CommitAsync();

            var (token, expirationDate) = _jwtTokenGenerator.GenerateToken(user);

            return new TokenDto
            {
                AccessToken = token,
                ExpiresAt = expirationDate
            };
        }

        public async Task<TokenDto> LoginAsync(LoginUserDto loginUserDto)
        {
            if (loginUserDto == null || string.IsNullOrWhiteSpace(loginUserDto.UsernameOrEmail) || string.IsNullOrWhiteSpace(loginUserDto.Password))
            {
                throw new BadRequestException("Invalid login data.");
            }

            var normalizedInput = loginUserDto.UsernameOrEmail.Trim().ToLower();
            var user = await _uow.UserRepository.GetByUsernameOrEmailAsync(normalizedInput);

            if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, loginUserDto.Password))
            {
                throw new BadRequestException("Invalid credentials.");
            }

            var (token, expirationDate) = _jwtTokenGenerator.GenerateToken(user);

            return new TokenDto
            {
                AccessToken = token,
                ExpiresAt = expirationDate
            };
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _uow.UserRepository.GetAllAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _uow.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _uow.UserRepository.GetAsync(id);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            _uow.UserRepository.Delete(user);
            await _uow.CommitAsync();
        }

        public async Task UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null)
            {
                throw new BadRequestException("User data is null");
            }

            var user = await _uow.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            user.Username = updateUserDto.Username;
            user.Email = updateUserDto.Email;

            _uow.UserRepository.Update(user);
            await _uow.CommitAsync();
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto == null)
            {
                throw new BadRequestException("Password change data is null");
            }

            var user = await _uow.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, changePasswordDto.CurrentPassword))
            {
                throw new BadRequestException("Current password is incorrect.");
            }

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                throw new BadRequestException("New passwords do not match.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(changePasswordDto.NewPassword);
            _uow.UserRepository.Update(user);
            await _uow.CommitAsync();
        }
    }

}
