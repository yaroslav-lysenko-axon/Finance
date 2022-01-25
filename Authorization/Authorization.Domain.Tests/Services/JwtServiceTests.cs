using System.Collections.Generic;
using System.Linq;
using Authorization.Domain.ConfigurationClasses;
using Authorization.Domain.Models;
using Authorization.Domain.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

#pragma warning disable CA2211, SA1401

namespace Authorization.Domain.Tests.Services
{
    public class JwtServiceTests
    {
        public static readonly string GetJwtStringTestData =
            "eyJhbGciOiJQUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI0MiIsImVtYWlsIjoidGVzdEBleGFtcGxlLmNvbSIsInVuaX" +
            "F1ZV9uYW1lIjoiSm9obiIsImZhbWlseV9uYW1lIjoiU21pdGgiLCJyb2xlIjoiVU5DT05GSVJNRURfVVNFUiIsInNjb3BlcyI6I" +
            "nNjb3BlLWdldC1vd24tcHJvZmlsZSIsIm5iZiI6MTYwMTQ3NDQwNSwiZXhwIjoxNjAxNDc4MDA1LCJpYXQiOjE2MDE0NzQ0MDUs" +
            "ImlzcyI6InRlc3QtaXNzdWVyIiwiYXVkIjoidGVzdC1hdXRob3JpdHkifQ.XHi_D4guahzoeiHGU7YQ-3H4aREpYKU5yYqqLj7w" +
            "b8ddTAsjh3RwB2kBpal_OOt4O7NYqNdB3OT9CG8RMUAqHuhENa4udckbjlDCNycA7JDaYhZPnCB74jsMHFnPX6BsEfsj9iJzId6" +
            "dJSX7zKw9g18VMeNMJBhzHXBmkvOhqHu2iY1Odk_hQK0m1khQ4lpljVqBnophpy2sCWnkG3rewbgXsAqzpnFqG-pQgqdUEf5zil" +
            "bf6oirqE-CMia9rZl7-SbqTIQQMZeHyJER77luBfrjkXnWZ2PaEtZ8uQT7R2iIlHstGX1zy6qbGct5KZUc3mZ5uZQuW5-Pr2AFH" +
            "yVrG6RYuWRiXwwT5wL4Ltjz7LEJZtuXGVPpJHzjw2iofkyRYPxzpWvRskU-u8OkTrarWy_jehtMZO2Ii0wOdD798rRHW6FkqmBK" +
            "L2VrvFAcrBLmMSk7hMPsXj8cp9FnDZWyQXmIhAt86Je_AEMccnA8Ply3AMg9qRquwMvasjwt-r_-";

        private readonly IJwtConfiguration _configuration;
        private readonly IUser _user;
        private readonly JwtService _sut;
        public JwtServiceTests()
        {
            _configuration = MockData.SetupDefaultJwtConfiguration();
            _user = MockData.SetupDefaultUser();
            var timeProvider = MockData.SetupDefaultTimeProvider();
            _sut = new JwtService(_configuration, timeProvider);
        }

        [Theory]
        [InlineData("issuer1")]
        [InlineData("issuer2")]
        public void CreateJwt_ChangingIssuerInConfiguration_SetsProperIssuer(string issuer)
        {
            var scopeIds = new List<string> { "scope-get-own-profile" };

            // Arrange
            _configuration.Issuer.Returns(issuer);

            // Act
            var jwt = _sut.CreateJwt(_user, scopeIds);

            // Assert
            jwt.Issuer.Should().Be(issuer);
        }

        [Theory]
        [InlineData("authority1")]
        [InlineData("authority2")]
        public void CreateJwt_ChangingAuthorityInConfiguration_SetsProperAuthority(string authority)
        {
            var scopeIds = new List<string> { "scope-get-own-profile" };

            // Arrange
            _configuration.Authority.Returns(authority);

            // Act
            var jwt = _sut.CreateJwt(_user, scopeIds);

            // Assert
            jwt.Audiences.Should().ContainSingle();
            jwt.Audiences.Single().Should().Be(authority);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(24)]
        public void CreateJwt_ChangingExpirationTimeInConfiguration_SetsProperValidTo(int expirationTimeInHours)
        {
            var scopeIds = new List<string> { "scope-get-own-profile" };

            // Arrange
            _configuration.ExpirationTimeInHours.Returns(expirationTimeInHours);

            // Act
            var jwt = _sut.CreateJwt(_user, scopeIds);

            // Assert
            (jwt.ValidTo - jwt.ValidFrom).TotalHours.Should().Be(expirationTimeInHours);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(200)]
        public void CreateJwt_ChangingExpirationTimeInConfiguration_SetsValidFromSameAsIssuedAt(int expirationTimeInHours)
        {
            var scopeIds = new List<string> { "scope-get-own-profile" };

            // Arrange
            _configuration.ExpirationTimeInHours.Returns(expirationTimeInHours);

            // Act
            var jwt = _sut.CreateJwt(_user, scopeIds);

            // Assert
            jwt.ValidFrom.Should().Be(jwt.IssuedAt);
        }

        [Fact]
        public void ValidateToken_WithJWT_ShouldBeTrue()
        {
            var scopeIds = new List<string> { "scope-get-own-profile" };

            // Arrange
            var jwtString = _sut.GetJwtString(_sut.CreateJwt(_user, scopeIds));

            // Act
            var jwtValidation = _sut.ValidateToken(jwtString);

            // Assert
            jwtValidation.Should().BeTrue();
        }

        [Fact]
        public void ValidateToken_WithExpiredJWT_ShouldBeFalse()
        {
            // Arrange
            var expiredJwtString = GetJwtStringTestData;

            // Act
            var jwtValidation = _sut.ValidateToken(expiredJwtString);

            // Assert
            jwtValidation.Should().BeFalse();
        }
    }
}

#pragma warning restore CA2211, SA1401
