using System;
using System.Threading.Tasks;
using Authorization.Domain.Models;

namespace Authorization.Domain.Services.Abstraction
{
    public interface IUserService
    {
        Task<User> RegisterUser(string email, string password, string firstName, string lastName);
        Task<User> GetUser(string email, string password);
        Task<User> GetUser(string email);
        Task<User> GetUserById(Guid userId);
        Task<User> GetInactiveUser(string email);
        Task UpdateUserProfile(User user);
        Task<User> ChangeUserPassword(Guid userId, string oldPassword, string newPassword);
    }
}
