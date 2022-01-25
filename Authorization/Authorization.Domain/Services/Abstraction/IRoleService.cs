using System.Threading.Tasks;
using Authorization.Domain.Models;

namespace Authorization.Domain.Services.Abstraction
{
    public interface IRoleService
    {
        Task<Role> FindByName(string roleName);
    }
}
