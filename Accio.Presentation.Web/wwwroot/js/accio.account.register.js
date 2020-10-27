const registerElements = {
    UsernameTextId: '#username',
    EmailTextId: '#emailAddress',
    PasswordTextId: '#password',
    SignUpButtonId: '#signUp',
    ErrorId: '#error',
    DataId: '#data',
};

$(document).ready(function () {
    InitializeElements();
});

function InitializeElements() {
    $(registerElements.SignUpButtonId).click(function (e) {
        e.preventDefault();
        SignUp();
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
        url: "Register?handler=Register",
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
                alert('account created');
            }
        },
        failure: function (response) {
            alert('Catastropic error');
        }
    });
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