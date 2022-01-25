namespace Authorization.Application.Extensions.Abstraction
{
    public interface ICookiesExtensions
    {
        public void SetTokenCookie(string refreshToken);
    }
}
