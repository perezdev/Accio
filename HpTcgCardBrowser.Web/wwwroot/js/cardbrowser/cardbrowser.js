var cardDeckId = '#cardContainer';

$(document).ready(function () {
    InitializeSearchElements();
    //SetValuesFromQuery();

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
    SetSearchLoadingState('loading');
    const searchData = GetSearchData();

    //Once the user executes a search, we want to set the values from the search to the query string so they can
    //refresh the page and/or share the link without the page needing to be refreshed
    SetQueryFromValues(searchData);

    var fd = new FormData();
    fd.append('setId', searchData.SetId);
    fd.append('lessonCost', searchData.LessonCost);
    fd.append('searchText', searchData.SearchText);

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

            SetSearchLoadingState('unloading');
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

//function SetValuesFromQuery() {
//    var baseUrl = window.location.href.split('?')[0];
//    history.pushState(null, null, baseUrl + '?text=test');
//}

function SetQueryFromValues(searchData) {
    //Only set the query string if at least one of the values have been set
    if (searchData.SetId || searchData.LessonCost || searchData.SearchText) {
        //If the existing URL has query string values, we need to ignore them so we don't add them to the existing ones.
        var baseUrl = window.location.href.split('?')[0];
        var queryValues = '?';

        if (searchData.SetId) {
            queryValues += searchData.SetId + '&';
        }
        if (searchData.LessonCost) {
            queryValues += searchData.LessonCost + '&';
        }
        if (searchData.SearchText) {
            queryValues += searchData.SearchText + '&';
        }

        //Since we don't know which fields the user will search, it's easiest to just to add & at the
        //end of each query value and lop the ending & once all have been set.
        queryValues = queryValues.slice('&', -1); 

        history.pushState(null, null, baseUrl + queryValues);
    }
}

//Default field values were causing issues with setting the query string from the field values
//We check the default values server side, but weren't doing anything client side
//This will clear the default values before setting the search data, which will
//allow us to properly check the query string and not set a value if it's false or default.
function GetSearchData() {
    var setId = $('#setSelect').val();
    var lessonCost = $('#setLessons').val();
    var searchText = $('#searchInput').val();

    if (setId === '00000000-0000-0000-0000-000000000000') {
        setId = null;
    }
    if (lessonCost === '-1') {
        lessonCost = null;
    }
    searchText = searchText.trim();

    const searchData = {
        SetId: setId,
        LessonCost: lessonCost,
        SearchText: searchText
    };

    return searchData;
}

function SetSearchLoadingState(loading) {
    if (loading === 'loading') {
        $('#searchButton').addClass('disabled');
        $('#searchButtonIcon').addClass('d-none');
        $('#searchButtonSpinner').removeClass('d-none');
    }
    else {
        $('#searchButton').removeClass('disabled');
        $('#searchButtonIcon').removeClass('d-none');
        $('#searchButtonSpinner').addClass('d-none');
    }
}