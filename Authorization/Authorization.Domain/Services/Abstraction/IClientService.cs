using System;
using System.Threading.Tasks;
using Authorization.Domain.Models;

namespace Authorization.Domain.Services.Abstraction
{
    public interface IClientService
    {
        Task<Client> AuthenticateClient(Guid clientId, Guid clientSecret);
        Task<Client> FindClient(Guid clientId);
    }
}
