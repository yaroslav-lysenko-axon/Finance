using System.Collections.Generic;
using System.Threading.Tasks;
using Authorization.Domain.Exceptions;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;

namespace Authorization.Domain.Services
{
    public class RoleScopeService : IRoleScopeService
    {
        private readonly IRoleScopeRepository _roleScopeRepository;

        public RoleScopeService(IRoleScopeRepository roleScopeRepository)
        {
            _roleScopeRepository = roleScopeRepository;
        }

        public async Task<List<RoleScope>> GetRoleScope(long role)
        {
            var roleScope = await _roleScopeRepository.GetRoleScope(role);

            if (roleScope == null)
            {
                throw new ClientNotAuthorizedException();
            }

            return roleScope;
        }
    }
}
