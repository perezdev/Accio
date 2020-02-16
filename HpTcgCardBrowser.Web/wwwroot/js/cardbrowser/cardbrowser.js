var cardDeckId = '#cardDeck';

$(document).ready(function () {
    InitializeSearchElements();
});

function InitializeSearchElements() {
    $('#searchButton').click(function () {
        SearchCards();
    });
    $('#searchInput').on('keypress', function (e) {
        if (e.which == 13) {
            SearchCards();
            e.preventDefault();
        }
    });
}

function SearchCards() {
    var setId = $('#setSelect').val();
    var lessonCost = $('#setLessons').val();
    var searchText = $('#searchInput').val();

    var fd = new FormData();
    fd.append('setId', setId);
    fd.append('lessonCost', lessonCost);
    fd.append('searchText', searchText);

    $.ajax({
        type: "POST",
        url: "CardBrowser?handler=SearchCards",
        data: fd,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        contentType: false,
        processData: false,
        success: function (response) {
            //ClearErrors(); //hide any errors from previous requests

            if (response.success) {
                var cards = response.json;
                AddCardsToDeck(cards);
            }
            else {
                //ShowErrors(response.message, response.info);
            }

            //SetDropDownMenuButtonLoadingState('unloading');
        },
        failure: function (response) {
            alert('Catastropic error');
        }
    });
}

function AddCardsToDeck(cards) {
    $(cardDeckId).html(''); //Clear existing cards

    for (var i = 0; i < cards.length; i++) {
        var card = cards[i];

        //Load the image directly so we can get the width
        //var cardImage = new Image();
        //cardImage.src = card.url;
        //$(cardImage).on('load', function () {
        //    //Add the card to deck after the image has loaded. The code will skip to the next
        //    //card while the image loads
        //    AddCardToDeck(card, cardImage.width);
        //});

        //Get all of the previous items in the card deck so we can add the new one
        var cardDeckHtml = $(cardDeckId).html();
        var cardElement = '<div class="card col-sm-3"><img src="' + card.url + '" /></div>';
        cardDeckHtml += cardElement;
        $(cardDeck).html(cardDeckHtml);
    }
}
function AddCardToDeck(card, cardWidth) {
    //Get all of the previous items in the card deck so we can add the new one
    var cardDeckHtml = $(cardDeckId).html();
    //var cardElement = '<div style="width: ' + cardWidth + 'px" class="card col-sm-3"><img src="' + card.url + '" /></div>';
    //cardDeckHtml += cardElement;
    //$(cardDeck).html(cardDeckHtml);
}