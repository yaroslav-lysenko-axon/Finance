using System;
using Authorization.Domain.ConfigurationClasses;
using Authorization.Domain.Models;
using Authorization.Domain.Services.Abstraction;
using NSubstitute;

namespace Authorization.Domain.Tests
{
    public static class MockData
    {
        public static IJwtConfiguration SetupDefaultJwtConfiguration()
        {
            var configuration = Substitute.For<IJwtConfiguration>();

            configuration.Issuer.Returns("test-issuer");
            configuration.Authority.Returns("test-authority");
            configuration.ExpirationTimeInHours.Returns(1);

            return configuration;
        }

        public static IUser SetupDefaultUser()
        {
            var user = GetDefaultUser();
            var userProxy = Substitute.For<IUser>();

            userProxy.Id.Returns(user.Id);
            userProxy.Email.Returns(user.Email);
            userProxy.PasswordHash.Returns(user.PasswordHash);
            userProxy.Avatar.Returns(user.Avatar);
            userProxy.Salt.Returns(user.Salt);
            userProxy.FirstName.Returns(user.FirstName);
            userProxy.LastName.Returns(user.LastName);
            userProxy.Role.Returns(user.Role);
            userProxy.Active.Returns(user.Active);

            return userProxy;
        }

        public static ITimeProvider SetupDefaultTimeProvider()
        {
            var timeProvider = Substitute.For<ITimeProvider>();

            timeProvider.UtcNow().Returns(DateTime.Now);
            return timeProvider;
        }

        public static Role GetDefaultRole()
        {
            return new Role { Id = 2, Name = "UNCONFIRMED_USER" };
        }

        public static User GetDefaultUser()
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "kBcsHwrVryh5WxniXVI/8oVT3QHPMOAi5adruNbwqYc=",
                Salt = "zmfk9Uxd6ggMHUNl16CnzQ==",
                Avatar = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Smith",
                Role = GetDefaultRole(),
                Active = false,
            };
        }
    }
}
