var cardDeckId = '#cardContainer';

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

        if (card.detail.url === undefined || card.detail.url === null) {
            continue;
        }

        var cardHtml = `
                        <div class="col-auto mb-3">
                            <div class="card ` + card.cssSizeClass + `">
                                <div class="card-body">
                                    <img class="d-none" id="` + card.cardId + `" data-cardname="` + card.detail.name + `" src="` + card.detail.url + `" />
                                </div>
                            </div>
                        </div>
                    `;

        //Get all of the previous items in the card deck so we can add the new one
        var cardDeckHtml = $(cardDeckId).html();
        cardDeckHtml += cardHtml;
        $(cardDeckId).html(cardDeckHtml);

        //When I first loaded the images, they would pop into view and it would look very jarring. All of the images load asnchorsouly, so it's nice not having to wait
        //for all the cards to load. But I wanted to animate it to look nicer. I do by hiding the image element as it's created. That allows the image to load before
        //it's shown. Then this will remove that class that hides it and animate a fade in from animate.css. The 'each' part is required and forces the load event to
        //fire when the image is loaded from cache. Which happens automatically by the browser.
        $('img').on('load', function () {
            $(this).removeClass('d-none');
            $(this).addClass('animated fadeIn');
        }).on('error', function () {
        }).each(function () {
            if (this.complete) {
                $(this).load();
            } else if (this.error) {
                $(this).error();
            }
        });
    }
}