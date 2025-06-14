using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Application.Interfaces
{
    public interface IUserClient
    {
        Task<UserDto?> GetUserByIdAsync(Guid userId);
    }
}
