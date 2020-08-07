const advancedSearchElements = {
    CardNameId: '#cardName',
    CardTextId: '#cardText',
};

function InitializeAdvancedPage() {
    /* Advanced search initialization */
    InitializeAdvancedSearchElements();

    /* Card search initialization */
    InitializeSearchBoxOnNonSearchPage();
}

function InitializeAdvancedSearchElements() {
    $(advancedSearchElements.CardNameId + ',' + advancedSearchElements.CardTextId).on('keypress', function (e) {
        if (e.which === 13) {
            RedirectToSearchWithAdvancedSearchString();
            e.preventDefault();
        }
    });
}

function RedirectToSearchWithAdvancedSearchString() {
    var cardName = $(advancedSearchElements.CardNameId).val();
    var cardText = $(advancedSearchElements.CardTextId).val();

    var fd = new FormData();
    if (cardName) {
        fd.append('cardName', cardName);
    }
    if (cardText) {
        fd.append('cardText', cardText);
    }

    $.ajax({
        type: "POST",
        url: "Advanced?handler=GetAdvancedSearchUrlValue",
        data: fd,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                var url = response.json;

                var baseUrl = location.protocol + '//' + location.host;
                var cardRoute = '/Search?adv=' + url;
                window.location.href = baseUrl + cardRoute;
            }
        },
        failure: function (response) {
            alert('Catastropic error');
        }
    });
}