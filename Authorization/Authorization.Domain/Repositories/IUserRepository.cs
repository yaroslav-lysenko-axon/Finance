using System;
using System.Threading.Tasks;
using Authorization.Domain.Models;

namespace Authorization.Domain.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> FindByEmail(string email);
        Task<User> FindById(Guid userId);
        Task<User> FindByEmailIgnoreCaseAndActiveFalse(string email);
        Task UpdateProfile(User user);
    }
}
