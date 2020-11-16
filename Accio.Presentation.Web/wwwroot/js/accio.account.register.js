const registerElements = {
    UsernameTextId: '#username',
    EmailTextId: '#emailAddress',
    PasswordTextId: '#password',
    SignUpButtonId: '#signUp',
    ErrorId: '#error',
    DataId: '#data',
    RegisterContainerId: '#registerContainer',
    VerifyContainerId: '#verifyContainer',
    VerifyEmailAddressId: '#verifyEmailAddress',
    VerificationCodeId: '#verificationCode',
    VerifyCodeId: '#verifyCode',
    SuccessContainerId: '#successContainer',
};

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

    var userName = $(registerElements.UsernameTextId).val();
    var emailAddress = $(registerElements.EmailTextId).val();
    var password = $(registerElements.PasswordTextId).val();
    var data = $(registerElements.DataId).val();

    if (!userName || !emailAddress || !password) {
        ShowRegisterErrors('User name, email address, or password is empty.');
        return;
    }

    var fd = new FormData();
    fd.append('userName', userName);
    fd.append('emailAddress', emailAddress);
    fd.append('password', password);
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
                ShowRegisterErrors(response.json);
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
function ShowRegisterErrors(message) {
    $(registerElements.ErrorId).html('');
    $(registerElements.ErrorId).html(message);
    $(registerElements.ErrorId).show();
}
function ClearRegisterErrors() {
    $(registerElements.ErrorId).html('');
    $(registerElements.ErrorId).hide();
}