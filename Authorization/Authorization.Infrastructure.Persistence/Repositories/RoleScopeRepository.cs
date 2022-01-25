using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Infrastructure.Persistence.Repositories
{
    public class RoleScopeRepository : GenericRepository<RoleScope>, IRoleScopeRepository
    {
        private readonly AuthContext _context;
        public RoleScopeRepository(AuthContext context)
            : base(context.RoleScopes)
        {
            _context = context;
        }

        public async Task<List<RoleScope>> GetRoleScope(long role)
        {
            var roleScopes = await _context.RoleScopes
                .Include(scope => scope.Scope)
                .Where(roleScope => roleScope.Role.Id == role)
                .ToListAsync().ConfigureAwait(false);
            return roleScopes;
        }
    }
}
