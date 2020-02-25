var cardDeckId = '#cardContainer';

const queryParameterNames = {
    SetId: 'setId',
    LessonCost: 'lessonCost',
    SearchText: 'searchText',
    SortBy: 'sortBy'
};

const searchElementNames = {
    SetId: '#setSelect',
    LessonCostId: '#setLessons',
    SearchTextId: '#searchInput',
    SearchButtonId: '#searchButton',
    SearchInputId: '#searchInput',
    SortCardsById: '#sortCardsBy'
};

$(document).ready(function () {
    InitializeSearchElements();
    SetValuesFromQueryAndPeformSearch();
});

function InitializeSearchElements() {
    $(searchElementNames.SearchButtonId).click(function () {
        SearchCards();
    });
    $(searchElementNames.SearchInputId).on('keypress', function (e) {
        if (e.which == 13) {
            SearchCards();
            e.preventDefault();
        }
    });
    $(searchElementNames.SortCardsById).on('change', function () {
        //I found an issue while testing. While not a huge problem, it made the experience weird. I wanted it so that
        //you could choose the sort option and it would perform a search. Which was perfect when you had already entered search data.
        //But it became troublesome when you hadn't entered anything, as it would just search everything. So I thought it best to make
        //it so that it would only perform the search when they chose a proper sort option (not the default empty) and there was at
        //least one search field with a valid value.
        var searchData = GetSearchData();
        if ((searchData.SetId || searchData.LessonCost || searchData.SearchText) && searchData.SortBy) {
            SearchCards();
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
    if (searchData.SetId) {
        fd.append('setId', searchData.SetId);
    }
    if (searchData.LessonCost) {
        fd.append('lessonCost', searchData.LessonCost);
    }
    if (searchData.SearchText) {
        fd.append('searchText', searchData.SearchText);
    }
    if (searchData.SortBy) {
        fd.append('sortBy', searchData.SortBy);
    }

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
            if (response.success) {
                var cards = response.json;
                AddCardsToDeck(cards);
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

async function SetValuesFromQueryAndPeformSearch() {
    var setId = getParameterByName(queryParameterNames.SetId);
    var lessonCost = getParameterByName(queryParameterNames.LessonCost);
    var searchText = getParameterByName(queryParameterNames.SearchText);
    //TODO: Add sort by

    if (lessonCost) {
        $(searchElementNames.LessonCostId).val(lessonCost);
    }
    if (searchText) {
        $(searchElementNames.SearchTextId).val(searchText);
    }

    if (setId) {
        //Set data comes from the database. We need to wait until it loads before we can
        //set the selected value, because the load is async and if we don't wait, there's
        //value to set
        await until(_ => hasSetsLoaded === true);
        $(searchElementNames.SetId).val(setId);
    }

    //This function is called on page load. If any of the query param values are passed in, we'll perform
    //the search
    if (setId || lessonCost || searchText) {
        SearchCards();
    }
}

function SetQueryFromValues(searchData) {
    //Only set the query string if at least one of the values have been set
    if (searchData.SetId || searchData.LessonCost || searchData.SearchText) {
        //If the existing URL has query string values, we need to ignore them so we don't add them to the existing ones.
        var baseUrl = window.location.href.split('?')[0];
        var queryValues = '?';

        if (searchData.SetId) {
            queryValues += queryParameterNames.SetId + '=' + searchData.SetId + '&';
        }
        if (searchData.LessonCost) {
            queryValues += queryParameterNames.LessonCost + '=' + searchData.LessonCost + '&';
        }
        if (searchData.SearchText) {
            queryValues += queryParameterNames.SearchText + '=' + searchData.SearchText + '&';
        }
        if (searchData.SortBy) {
            queryValues += queryParameterNames.SortBy + '=' + searchData.SortBy + '&';
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
    var setId = $(searchElementNames.SetId).val();
    var lessonCost = $(searchElementNames.LessonCostId).val();
    var searchText = $(searchElementNames.SearchTextId).val();
    var sortBy = $(searchElementNames.SortCardsById).val();

    if (setId === '00000000-0000-0000-0000-000000000000') {
        setId = null;
    }
    if (lessonCost === '-1') {
        lessonCost = null;
    }
    searchText = searchText.trim();
    if (sortBy === 'NoSort') {
        sortBy = null;
    }

    const searchData = {
        SetId: setId,
        LessonCost: lessonCost,
        SearchText: searchText,
        SortBy: sortBy
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