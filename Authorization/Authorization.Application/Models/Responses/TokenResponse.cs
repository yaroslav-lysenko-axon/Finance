namespace Authorization.Application.Models.Responses
{
    public class TokenResponse
    {
        public AccessTokenResponse AccessTokenResponse { get; set; }
        public RefreshTokenResponse RefreshTokenResponse { get; set; }
    }
}
