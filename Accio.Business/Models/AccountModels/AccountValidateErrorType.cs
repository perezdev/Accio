namespace Accio.Business.Models.AccountModels
{
    public enum AccountValidateErrorType
    {
        EmailAddressEmpty,
        EmailAddressInvalidFormat,
        EmailAddressAlreadyExists,
        PasswordEmpty,
        PasswordTooShort,
        PasswordNotComplicatedEnough,
        UsernameEmpty,
        UsernameExists,
        ConfirmPasswordInvalid
    }
}
