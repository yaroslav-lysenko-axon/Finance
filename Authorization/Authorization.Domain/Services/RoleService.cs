using System.Threading.Tasks;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;

namespace Authorization.Domain.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Role> FindByName(string roleName)
        {
            var role = await _roleRepository.FindByName(roleName);
            return role;
        }
    }
}
