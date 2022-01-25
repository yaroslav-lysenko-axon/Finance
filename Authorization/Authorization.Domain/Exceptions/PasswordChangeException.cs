namespace Authorization.Domain.Exceptions
{
    /// <summary>
    /// PasswordChangeException <see cref="PasswordChangeException"/> class.
    /// </summary>
    public class PasswordChangeException : ApplicationException
    {
        private const string DuplicatePassword =
            "The old and new passwords are the same. Please enter a different new password.";

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordChangeException"/> class.
        /// </summary>
        public PasswordChangeException()
            : base(Models.ErrorCode.DuplicatePassword,  System.Net.HttpStatusCode.BadRequest, DuplicatePassword)
        {
        }
    }
}
