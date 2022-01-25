#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using Authorization.Domain.ConfigurationClasses;
using Authorization.Domain.Models;
using Authorization.Domain.Services.Abstraction;
using Microsoft.IdentityModel.Tokens;
using PemUtils;

namespace Authorization.Domain.Services
{
    public class JwtService : IJwtService
    {
        private readonly IJwtConfiguration _jwtConfiguration;
        private readonly ITimeProvider _timeProvider;

        public JwtService(
            IJwtConfiguration jwtConfiguration,
            ITimeProvider timeProvider)
        {
            _jwtConfiguration = jwtConfiguration;
            _timeProvider = timeProvider;
        }

        public JwtSecurityToken CreateJwt(IUser user, List<string> scopeIds)
        {
            var pathPrivateKey = Directory.GetCurrentDirectory() + "/private-key.pem";
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = CreateClaimsIdentity(user, scopeIds);
            var now = _timeProvider.UtcNow();

            RsaSecurityKey rsaKey;
            using (var stream = File.OpenRead(pathPrivateKey))
            using (var reader = new PemReader(stream))
            {
                rsaKey = new RsaSecurityKey(reader.ReadRsaKey());
            }

            var signingCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSsaPssSha256);
            var token = tokenHandler.CreateJwtSecurityToken(
                issuer: _jwtConfiguration.Issuer,
                audience: _jwtConfiguration.Authority,
                subject: claims,
                notBefore: now,
                expires: now.AddHours(_jwtConfiguration.ExpirationTimeInHours),
                issuedAt: now,
                signingCredentials: signingCredentials);

            return token;
        }

        public string GetJwtString(JwtSecurityToken token) => new JwtSecurityTokenHandler().WriteToken(token);
        public bool ValidateToken(string token)
        {
            var pathPublicKey = Directory.GetCurrentDirectory() + "/public-key.pem";
            byte[] publicKey;
            using (var publicKeyFileStream = new StreamReader(pathPublicKey))
            {
                var publicKeyString = publicKeyFileStream.ReadToEnd()
                    .Replace("-----BEGIN PUBLIC KEY-----", " ", StringComparison.CurrentCulture)
                    .Replace("-----END PUBLIC KEY-----", " ", StringComparison.CurrentCulture);
                publicKey = Convert.FromBase64String(publicKeyString);
            }

            using RSA rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(publicKey, out _);

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _jwtConfiguration.Issuer,
                        ValidAudience = _jwtConfiguration.Authority,
                        IssuerSigningKey = new RsaSecurityKey(rsa),
                        CryptoProviderFactory = new CryptoProviderFactory()
                        {
                            CacheSignatureProviders = false,
                        },
                    },
                    out _);

                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }

        private static ClaimsIdentity CreateClaimsIdentity(IUser user, List<string> scopeIds)
        {
            var claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture.ToString())));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role.Name));
            foreach (var scopeId in scopeIds)
            {
                claimsIdentity.AddClaim(new Claim("scopes", scopeId));
            }

            return claimsIdentity;
        }
    }
}
