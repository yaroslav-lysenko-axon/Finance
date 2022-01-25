using System.Threading.Tasks;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Infrastructure.Persistence.Contexts;

namespace Authorization.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AuthContext context)
            : base(context.Roles)
        {
        }

        public async Task<Role> FindByName(string roleName)
        {
            return await FindFirst(role => role.Name == roleName);
        }
    }
}
