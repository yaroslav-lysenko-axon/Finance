using System.ComponentModel.DataAnnotations;

namespace Authorization.Domain.Models
{
    public enum ErrorCode
    {
        [Display(Name = "error_client_authorization_failed")]
        ClientAuthorizationFailed,
        [Display(Name = "error_role_not_found")]
        RoleNotFound,
        [Display(Name = "error_duplicated_email")]
        DuplicateMail,
        [Display(Name = "error_validation_failed")]
        ValidationFailed,
        [Display(Name = "error_invalid_password")]
        InvalidPassword,
        [Display(Name = "error_old_and_new_passwords_are_the_same")]
        DuplicatePassword,
        [Display(Name = "error_user_not_found")]
        UserNotFound,
        [Display(Name = "confirmation_request_not_found")]
        ConfirmationRequestNotFound,
        [Display(Name = "user_email_already_confirmed")]
        UserEmailAlreadyConfirmed,
        [Display(Name = "confirmation_request_expired")]
        ConfirmationRequestExpired,
    }
}
