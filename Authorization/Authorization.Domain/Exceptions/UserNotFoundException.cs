using System;

namespace Authorization.Domain.Exceptions
{
    /// <summary>
    /// UserNotFoundException <see cref="UserNotFoundException"/> class.
    /// </summary>
    public class UserNotFoundException : ApplicationException
    {
        private const string UserIdNotFound = "Not found user by UserId: ";
        private const string UserEmailNotFound = "Not found user by email: ";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="userId"> User Id.</param>
        public UserNotFoundException(Guid userId)
            : base(Models.ErrorCode.UserNotFound, System.Net.HttpStatusCode.NotFound, UserIdNotFound + userId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="email"> User email.</param>
        public UserNotFoundException(string email)
            : base(Models.ErrorCode.UserNotFound, System.Net.HttpStatusCode.NotFound, UserEmailNotFound + email)
        {
        }
    }
}
