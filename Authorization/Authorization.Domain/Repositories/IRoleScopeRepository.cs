using System.Collections.Generic;
using System.Threading.Tasks;
using Authorization.Domain.Models;

namespace Authorization.Domain.Repositories
{
    public interface IRoleScopeRepository : IGenericRepository<RoleScope>
    {
        Task<List<RoleScope>> GetRoleScope(long role);
    }
}
