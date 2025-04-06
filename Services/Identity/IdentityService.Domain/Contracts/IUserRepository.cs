using IdentityService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<User> GetByIdAsync(Guid userId);
        Task<User> GetAsync(Guid id);
        Task InsertAsync(User user);
        Task<bool> UserExistsAsync(string email);
        Task<List<User>> GetAllAsync(); // Dodajemy metodę GetAllAsync
        Task SaveChangesAsync();
        void Update(User user);
        void Delete(User user);
    }

}
