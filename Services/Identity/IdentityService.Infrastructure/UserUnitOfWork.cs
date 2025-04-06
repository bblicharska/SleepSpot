using IdentityService.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure
{
    public class UserUnitOfWork : IUserUnitOfWork
    {
        private readonly UserDbContext _context;

        public IUserRepository UserRepository { get; }


        public UserUnitOfWork(UserDbContext context, IUserRepository userRepository)
        {
            _context = context;
            this.UserRepository = userRepository;
        }
        public void Commit()
        {
            _context.SaveChanges();
        }
        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
