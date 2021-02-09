const registerElements = {
    UsernameTextId: '#username',
    EmailTextId: '#emailAddress',
    PasswordTextId: '#password',
    SignUpButtonId: '#signUp',
    ErrorContainerId: '#errorContainer',
    ErrorId: '#error',
    DataId: '#data',
    RegisterContainerId: '#registerContainer',
    VerifyContainerId: '#verifyContainer',
    VerifyEmailAddressId: '#verifyEmailAddress',
    VerificationCodeId: '#verificationCode',
    VerifyCodeId: '#verifyCode',
    SuccessContainerId: '#successContainer',
    EmailAddressErrorMessageId: '#emailAddressErrorMessage',
    UsernameErrorMessageId: '#usernameErrorMessage',
    PasswordErrorMessageId: '#passwordErrorMessage',
    ConfirmErrorMessageId: '#confirmErrorMessage',
    ConfirmPasswordTextId: '#confirmPassword',
    RegisterFormInputErrorClassName: 'register-form-input-error',
    PasswordNotComplicatedEnoughErrorMessageId: '#passwordNotComplicatedEnoughErrorMessage',
    EmailAddressAlreadyExistsErrorMessageId: '#emailAddressAlreadyExistsErrorMessage',
    UsernameAlreadyExistsErrorMessageId: '#usernameAlreadyExistsErrorMessage'
};

const AccountValidateErrorType = {
    EmailAddressEmpty: 0,
    EmailAddressInvalidFormat: 1,
    EmailAddressAlreadyExists: 2,
    PasswordEmpty: 3,
    PasswordTooShort: 4,
    PasswordNotComplicatedEnough: 5,
    UsernameEmpty: 6,
    UsernameExists: 7,
    ConfirmPasswordInvalid: 8
}

$(document).ready(function () {
    InitializeRegisterElements();
});

function InitializeRegisterElements() {
    $(registerElements.SignUpButtonId).click(function (e) {
        e.preventDefault();
        SignUp();
    });

    $(registerElements.VerifyCodeId).click(function (e) {
        e.preventDefault();
        VerifyCode();
    });
}

function SignUp() {
    ClearRegisterErrors();

    var emailAddress = $(registerElements.EmailTextId).val();
    var username = $(registerElements.UsernameTextId).val();
    var password = $(registerElements.PasswordTextId).val();
    var confirmPassword = $(registerElements.ConfirmPasswordTextId).val();
    var data = $(registerElements.DataId).val();

    var fd = new FormData();
    fd.append('userName', username);
    fd.append('emailAddress', emailAddress);
    fd.append('password', password);
    fd.append('confirmPassword', confirmPassword);
    fd.append('data', data);

    $.ajax({
        type: "POST",
        url: "?handler=Register",
        traditional: true,
        data: fd,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                TransitionToVerify();
            }
            else {
                console.log(response.json);
                ShowRegisterValidationErrors(response.json);
            }
        },
        failure: function (response) {
            alert('Catastropic error');
        }
    });
}

function VerifyCode() {
    ClearRegisterErrors();

    var emailAddress = $(registerElements.EmailTextId).val();
    var code = $(registerElements.VerificationCodeId).val();

    if (!emailAddress || !code) {
        ShowRegisterErrors('Email address and code are required.');
        return;
    }

    var fd = new FormData();
    fd.append('emailAddress', emailAddress);
    fd.append('code', code);

    $.ajax({
        type: "POST",
        url: "?handler=VerifyCode",
        traditional: true,
        data: fd,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                TransitionToRegisterSuccess();
            }
            else {
                ShowRegisterErrors(response.json);
            }
        },
        failure: function (response) {
            alert('Catastropic error');
        }
    });
}
function TransitionToVerify() {
    $(registerElements.RegisterContainerId).addClass('dn');
    $(registerElements.VerifyContainerId).removeClass('dn');
}
function TransitionToRegisterSuccess() {
    $(registerElements.VerifyContainerId).addClass('dn');
    $(registerElements.SuccessContainerId).removeClass('dn');
}
function ShowRegisterValidationErrors(errors) {
    for (var i = 0; i < errors.length; i++) {
        var n = errors[i];

        if (n === AccountValidateErrorType.EmailAddressEmpty || n === AccountValidateErrorType.EmailAddressInvalidFormat) {
            $(registerElements.EmailAddressErrorMessageId).show();
            $(registerElements.EmailTextId).addClass(registerElements.RegisterFormInputErrorClassName);
        }
        else if (n === AccountValidateErrorType.EmailAddressAlreadyExists) {
            $(registerElements.EmailAddressAlreadyExistsErrorMessageId).show();
            $(registerElements.EmailTextId).addClass(registerElements.RegisterFormInputErrorClassName);
        }
        else if (n === AccountValidateErrorType.PasswordEmpty) {
            $(registerElements.PasswordErrorMessageId).show();
            $(registerElements.PasswordTextId).addClass(registerElements.RegisterFormInputErrorClassName);
        }
        else if (n === AccountValidateErrorType.PasswordNotComplicatedEnough) {
            $(registerElements.PasswordNotComplicatedEnoughErrorMessageId).show();
            $(registerElements.PasswordTextId).addClass(registerElements.RegisterFormInputErrorClassName);
        }
        else if (n === AccountValidateErrorType.UsernameEmpty) {
            $(registerElements.UsernameErrorMessageId).show();
            $(registerElements.UsernameTextId).addClass(registerElements.RegisterFormInputErrorClassName);
        }
        else if (n === AccountValidateErrorType.UsernameExists) {
            $(registerElements.UsernameAlreadyExistsErrorMessageId).show();
            $(registerElements.UsernameTextId).addClass(registerElements.RegisterFormInputErrorClassName);
        }
        else if (n === AccountValidateErrorType.ConfirmPasswordInvalid) {
            $(registerElements.ConfirmErrorMessageId).show();
            $(registerElements.ConfirmPasswordTextId).addClass(registerElements.RegisterFormInputErrorClassName);
        }
    }
}
function ShowRegisterErrors(message) {
    $(registerElements.ErrorId).html('');
    $(registerElements.ErrorId).html(message);
    $(registerElements.ErrorId).show();
}
function ClearRegisterErrors() {
    $(registerElements.ErrorId).html('');
    $(registerElements.ErrorId).hide();

    $(registerElements.EmailAddressErrorMessageId).hide();
    $(registerElements.UsernameErrorMessageId).hide();
    $(registerElements.PasswordErrorMessageId).hide();
    $(registerElements.ConfirmErrorMessageId).hide();
    $(registerElements.PasswordNotComplicatedEnoughErrorMessageId).hide();
    $(registerElements.EmailAddressAlreadyExistsErrorMessageId).hide();
    $(registerElements.UsernameAlreadyExistsErrorMessageId).hide();

    $(registerElements.EmailTextId).removeClass(registerElements.RegisterFormInputErrorClassName);
    $(registerElements.UsernameTextId).removeClass(registerElements.RegisterFormInputErrorClassName);
    $(registerElements.PasswordTextId).removeClass(registerElements.RegisterFormInputErrorClassName);
    $(registerElements.ConfirmPasswordTextId).removeClass(registerElements.RegisterFormInputErrorClassName);
}