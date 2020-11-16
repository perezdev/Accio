const loginElements = {
    LoginButtonId: '#login',
    EmailTextId: '#emailAddress',
    PasswordTextId: '#password',
    LoginContainerId: '#loginContainer',
};

$(document).ready(function () {
    InitializeLoginElements();
});

function InitializeLoginElements() {
    $(loginElements.LoginButtonId).click(function (e) {
        e.preventDefault();
        Login();
    });
}

function Login() {
    ClearLoginErrors();

    var emailAddress = $(loginElements.EmailTextId).val();
    var password = $(loginElements.PasswordTextId).val();

    if (!emailAddress || !password) {
        ShowLoginErrors('Email address or password is empty.');
        return;
    }

    var fd = new FormData();
    fd.append('emailAddress', emailAddress);
    fd.append('password', password);

    $.ajax({
        type: "POST",
        url: "?handler=Login",
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
                location.reload();
            }
            else {
                ShowLoginErrors(response.json);
            }
        },
        failure: function (response) {
            alert('Catastropic error');
        }
    });
}

function ShowLoginErrors(message) {
    $(registerElements.ErrorId).html('');
    $(registerElements.ErrorId).html(message);
    $(registerElements.ErrorId).show();
}
function ClearLoginErrors() {
    $(registerElements.ErrorId).html('');
    $(registerElements.ErrorId).hide();
}