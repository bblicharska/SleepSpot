using IdentityService.Domain.Contracts;
using IdentityService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
        {
            private readonly UserDbContext _context;

            public UserRepository(UserDbContext context)
            {
                _context = context;
            }

            public async Task<User> GetAsync(Guid id)
            {
                return await _context.Users.FindAsync(id);
            }

            public async Task<List<User>> GetAllAsync() // Zmieniono IList<User> na List<User>
            {
                return await _context.Users.ToListAsync();
            }

            public async Task InsertAsync(User user)
            {
                await _context.Users.AddAsync(user);
            }

            public void Update(User user) // Zmieniono na metodę synchroniczną
            {
                _context.Users.Update(user);
            }

            public void Delete(User user) // Zmieniono na metodę synchroniczną
            {
                _context.Users.Remove(user);
            }

            public async Task<IList<User>> FindAsync(Expression<Func<User, bool>> predicate)
            {
                return await _context.Users.Where(predicate).ToListAsync();
            }

            public async Task<User> GetByUsernameOrEmailAsync(string usernameOrEmail)
            {
                return await _context.Users
                    .FirstOrDefaultAsync(user => user.Username == usernameOrEmail || user.Email == usernameOrEmail);
            }

            public async Task<User> GetByIdAsync(Guid userId)
            {
                return await _context.Users.FindAsync(userId);
            }

            public async Task<bool> UserExistsAsync(string email)
            {
                return await _context.Users.AnyAsync(user => user.Email == email);
            }

            public async Task SaveChangesAsync()
            {
                await _context.SaveChangesAsync();
            }
        }
 
}
