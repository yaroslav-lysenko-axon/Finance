using System;
using System.Linq;
using System.Threading.Tasks;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AuthContext _context;
        public UserRepository(AuthContext context)
            : base(context.Users)
        {
            _context = context;
        }

        public async Task<User> FindByEmail(string email)
        {
            return await _context.Users
                .Include(user => user.Role)
                .Where(user => user.Email == email)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<User> FindById(Guid userId)
        {
            return await _context.Users
                .Include(user => user.Role)
                .Where(user => user.Id == userId)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<User> FindByEmailIgnoreCaseAndActiveFalse(string email)
        {
            return await _context.Users
                .Include(user => user.Role)
                .Where(user => user.Email == email)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public Task UpdateProfile(User user)
        {
            _context.Set<User>().UpdateRange(user);
            return Task.CompletedTask;
        }
    }
}
