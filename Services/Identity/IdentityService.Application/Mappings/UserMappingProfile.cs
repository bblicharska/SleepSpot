using AutoMapper;
using IdentityService.Application.Dto;
using IdentityService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IdentityService.Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<RegisterUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Haszowanie hasła nie jest obsługiwane tutaj
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)) // Dodanie CreatedAt
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "User")); // Domyślna rola użytkownika

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Ignorowanie hasła
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Ignorowanie CreatedAt
                .ForMember(dest => dest.Role, opt => opt.Ignore()); // Ignorowanie roli

            CreateMap<TokenDto, object>();

            CreateMap<LoginUserDto, User>();

            CreateMap<ChangePasswordDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Haszowanie hasła w UserService
        }
    }
}
