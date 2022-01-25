using System.Collections.Generic;
using System.Threading.Tasks;
using Authorization.Domain.Models;

namespace Authorization.Domain.Services.Abstraction
{
    public interface IRoleScopeService
    {
        Task<List<RoleScope>> GetRoleScope(long role);
    }
}
