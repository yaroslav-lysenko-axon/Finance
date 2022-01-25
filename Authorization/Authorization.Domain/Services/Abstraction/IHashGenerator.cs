namespace Authorization.Domain.Services.Abstraction
{
    public interface IHashGenerator
    {
        string CreateSalt();
        string GenerateHash(string valueToHash, string salt);
        bool VerifyPassword(string oldPassword, string saltFromDb, string passwordHashFromDb);
    }
}
