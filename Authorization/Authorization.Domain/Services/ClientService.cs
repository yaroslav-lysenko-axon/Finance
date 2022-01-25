using System;
using System.Threading.Tasks;
using Authorization.Domain.Exceptions;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;

namespace Authorization.Domain.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Client> AuthenticateClient(Guid clientId, Guid clientSecret)
        {
            var client = await _clientRepository.FindFirst(x => x.Id == clientId && x.ClientSecret == clientSecret);

            if (client == null)
            {
                throw new ClientNotAuthorizedException();
            }

            return client;
        }

        public async Task<Client> FindClient(Guid clientId)
        {
            var client = await _clientRepository.FindFirst(client => client.Id == clientId);

            if (client == null)
            {
                throw new ClientNotAuthorizedException();
            }

            return client;
        }
    }
}
