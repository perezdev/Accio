const loginElements = {
    LoginButtonId: '#login',
    EmailTextId: '#emailAddress',
    PasswordTextId: '#password',
};

$(document).ready(function () {
    InitializeElements();
});

function InitializeElements() {
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

    alert('test');
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