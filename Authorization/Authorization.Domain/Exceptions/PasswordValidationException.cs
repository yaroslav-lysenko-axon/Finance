namespace Authorization.Domain.Exceptions
{
    /// <summary>
    /// PasswordValidationException <see cref="PasswordValidationException"/> class.
    /// </summary>
    public class PasswordValidationException : ApplicationException
    {
        private const string InvalidPassword = "Wrong password.";

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordValidationException"/> class.
        /// </summary>
        public PasswordValidationException()
            : base(Models.ErrorCode.InvalidPassword,  System.Net.HttpStatusCode.InternalServerError, InvalidPassword)
        {
        }
    }
}
