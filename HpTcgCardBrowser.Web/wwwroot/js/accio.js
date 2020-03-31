/**
 * On page load
 */
$(document).ready(function () {
    /* Card set initialization */
    SetCardSets();

    /* Card search initialization */
    InitializeCardSearchElements();
    SetValuesFromQueryAndPeformCardSearch();
    InitializeCardTable();
});

/**
 * Card Sets
 * -----------------------------------------------------------------------------------------------------
 */

//We need to complete some activities after the sets have loaded
//Since that process isn't always fast, we'll set this variable and check it
var hasSetsLoaded = false;

function SetCardSets() {
    $.ajax({
        type: "POST",
        url: "Search?handler=GetSets",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        contentType: false,
        processData: false,
        success: function (response) {
            //ClearErrors(); //hide any errors from previous requests

            if (response.success) {
                var sets = response.json;
                AddSetsToDropDown(sets);
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
function AddSetsToDropDown(sets) {
    var dropDownName = '#setSelect';

    //Clear all options before adding new ones, just in case
    $(dropDownName).find('option').remove();

    //Add empty item so the user can choose to not search by set if they want
    var emptyOption = '<option value="00000000-0000-0000-0000-000000000000">All Sets</option>';
    $(dropDownName).append(emptyOption);

    for (var i = 0; i < sets.length; i++) {
        var set = sets[i];
        var img = '/images/seticons/' + set.iconFileName;

        var option = '<option value="' + set.cardSetId + '">' + set.name + '</option>';
        $(dropDownName).append(option);
    }

    hasSetsLoaded = true;
}

/**
 * Card Search
 * -----------------------------------------------------------------------------------------------------
 */

const queryParameterNames = {
    SetId: 'setId',
    SearchText: 'searchText',
    SortBy: 'sortBy',
    SortOrder: 'sortOrder',
    CardView: 'cardView'
};

const searchElementNames = {
    SetId: '#setSelect',
    SearchInputId: '#searchInput',
    SortCardsById: '#sortCardsBy',
    CardModalId: '#cardModal',
    SortCardsOrderId: '#sortCardsOrder',
    CardViewId: '#cardView'
};

const resultsContainerNames = {
    ContentContainerId: '#contentContainer',
    CardContainerId: '#cardContainer',
    CardTableId: '#cardTable',
    CardTableContainerId: '#tableContainer'
};

/* Hold cards in global variable so we can swap views with the existing cards without
 * performing another query.
 */
var cards = null;

function InitializeCardSearchElements() {
    //Search text press enter
    $(searchElementNames.SearchInputId).on('keypress', function (e) {
        if (e.which === 13) {
            SearchCards();
            e.preventDefault();
        }
    });
    //Set change
    $(searchElementNames.SetId).on('change', function () {
        SearchCards();
    });
    //Card view change - images/list
    $(searchElementNames.CardViewId).on('change', function () {
        //Will swap the containers and populate the cards if they have been populated. Or performs the search if the cards haven't been populated
        ToggleViewSearch();
    });
    //Sort by change
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
    //TODO: sort order change
}
var cardTable = null;
function InitializeCardTable() {
    cardTable = $(resultsContainerNames.CardTableId).DataTable({
        lengthChange: false,
        searching: false,
        pageLength: 15
    });
}

function SearchCards() {
    //SetSearchLoadingState('loading');
    const searchData = GetSearchData();

    //Once the user executes a search, we want to set the values from the search to the query string so they can
    //refresh the page and/or share the link without the page needing to be refreshed
    SetQueryFromValues(searchData);

    var fd = new FormData();
    if (searchData.SetId) {
        fd.append('setId', searchData.SetId);
    }
    if (searchData.SearchText) {
        fd.append('searchText', searchData.SearchText);
    }
    if (searchData.SortBy) {
        fd.append('sortBy', searchData.SortBy);
    }
    if (searchData.SortBy) {
        fd.append('sortOrder', searchData.SortOrder);
    }

    $.ajax({
        type: "POST",
        url: "Search?handler=SearchCards",
        data: fd,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                cards = response.json;
                AddCardsToContainer(cards);
            }

            //SetSearchLoadingState('unloading');
        },
        failure: function (response) {
            alert('Catastropic error');
        }
    });
}
//Adds cards to selected container if the cards have already been searched
//Searches cards and adds them otherwise
function ToggleViewSearch() {
    if (cards !== null) {
        //Normally we'd only get the searchdata and update the query values while searching
        //But in the case where the card data already exists, we want to update the query data because
        //we won't perform a search.
        const searchData = GetSearchData();
        SetQueryFromValues(searchData);

        AddCardsToContainer(cards);
    }
    else {
        SearchCards();
    }
}
function ToggleViewContainers() {
    var cardView = $(searchElementNames.CardViewId).val();
    if (cardView === 'images') {
        $(resultsContainerNames.CardContainerId).css('display', 'flex');
        $(resultsContainerNames.CardTableContainerId).css('display', 'none');
    } else if (cardView === 'checklist') {
        $(resultsContainerNames.CardContainerId).css('display', 'none');
        $(resultsContainerNames.CardTableContainerId).css('display', 'block');
    }
}

//Makes the current selected container visible and then adds the cards to that container
function AddCardsToContainer(cards) {
    ToggleViewContainers();
    var cardView = $(searchElementNames.CardViewId).val();
    if (cardView === 'images') {
        AddCardsToDeck(cards);
    } else if (cardView === 'checklist') {
        AddCardsToTable(cards);
    }
}
function AddCardsToDeck(cards) {
    //TODO: Hide/show containers

    //Clear existing cards
    $(resultsContainerNames.CardContainerId).html('');

    for (var i = 0; i < cards.length; i++) {
        var card = cards[i];

        if (card.detail.url === undefined || card.detail.url === null) {
            continue;
        }

        var cardHtml = `
                        <div onclick="ShowCardModal('` + card.cardId + `');" class="ma1 ` + card.cssSizeClass + `">
                            <img class="tc" style="cursor: pointer;" id="` + card.cardId + `" data-cardname="` + card.detail.name + `" src="` + card.detail.url + `" />
                        </div>
                    `;

        //Get all of the previous items in the card deck so we can add the new one
        var cardDeckHtml = $(resultsContainerNames.CardContainerId).html();
        cardDeckHtml += cardHtml;
        $(resultsContainerNames.CardContainerId).html(cardDeckHtml);

        //When I first loaded the images, they would pop into view and it would look very jarring. All of the images load asnchorsouly, so it's nice not having to wait
        //for all the cards to load. But I wanted to animate it to look nicer. I do this by hiding the image element as it's created. That allows the image to load before
        //it's shown. Then this will remove that class that hides it and animate a fade in from animate.css. The 'each' part is required and forces the load event to
        //fire when the image is loaded from cache. Which happens automatically by the browser.
        $('img').on('load', function () {
            $(this).removeClass('d-none');
            $(this).addClass('animated fadeIn');
        }).on('error', function () {
        }).each(function () {
            if (this.complete) {
                $(this).trigger('load');
            } else if (this.error) {
                $(this).trigger('error');
            }
        });
    }
}
function AddCardsToTable(cards) {
    //Remove all cards prior to adding any new ones from search
    cardTable.clear().draw();

    for (var i = 0; i < cards.length; i++) {
        var card = cards[i];

        var costValue = card.lessonCost === null ? '' : card.lessonCost;
        var setColumn = '<td>' + card.cardSet.shortName + '</td>';
        var cardNumberColumn = '<td>' + card.cardNumber + '</td>';
        var cardNameColumn = '<td>' + card.detail.name + '</td>';

        var costColumn = null;
        if (card.lessonType === null) {
            costColumn = costColumn = '<td>' + costValue + '</td>';
        }
        else if (card.lessonType !== null && costValue !== '') {
            var imgElement = GetLessonImageElementFromLessonType(card.lessonType.name);
            costColumn = costColumn = '<td><label class="card-table-cell-lesson-label">' + costValue + '</label>' + imgElement + '</td>';
        }

        var typeColumn = '<td>' + card.cardType.name + '</td>';

        var rarityColumn = null;
        if (card.rarity.imageName === null) {
            rarityColumn = '<td>' + card.rarity.symbol + '</td>';
        }
        else {
            var rarityImage = GetRarityImageElementFromRaritySymbol(card.rarity.symbol);
            rarityColumn = '<td>' + card.rarity.symbol + rarityImage + '</td>';
        }

        var artistColumn = '<td>' + card.detail.illustrator + '</td>';

        //Add row to table. Passing in a comma separated list for each column will add the columns in that order.
        //The second column is hidden by the column definitions when the table was instantiated
        var rowNode = cardTable.row.add([
            setColumn, cardNumberColumn, cardNameColumn, costColumn, typeColumn, rarityColumn, artistColumn
        ]).draw().node();

        //The design calls for changing the color of the font and can really only be done after the fact. DT.js
        //overwrites style changes when made as part of the column html.
        if (card.lessonType !== null) {
            var lessonCssColor = GetLessonCssColorFromLessonType(card.lessonType.name);
            $(rowNode).find('td').eq(3).css('color', lessonCssColor);
        }
    }
}
function GetLessonCssColorFromLessonType(lessonType) {
    if (lessonType === 'Care of Magical Creatures') {
        return 'var(--brownPaw)';
    }
    else if (lessonType === 'Charms') {
        return 'var(--blueRaven)';
    }
    else if (lessonType === 'Potions') {
        return 'var(--greenSnake)';
    }
    else if (lessonType === 'Quidditch') {
        return 'var(--yellowBadger)';
    }
    else if (lessonType === 'Transfiguration') {
        return 'var(--redLion)';
    }
}
function GetLessonImageElementFromLessonType(lessonType) {
    if (lessonType === 'Care of Magical Creatures') {
        return '<img class="card-table-cell-lesson-image" src="/images/lessons/care-of-magical-creatures.svg" />';
    }
    else if (lessonType === 'Charms') {
        return '<img class="card-table-cell-lesson-image" src="/images/lessons/charms.svg" />';
    }
    else if (lessonType === 'Potions') {
        return '<img class="card-table-cell-lesson-image" src="/images/lessons/potions.svg" />';
    }
    else if (lessonType === 'Quidditch') {
        return '<img class="card-table-cell-lesson-image" src="/images/lessons/quidditch.svg" />';
    }
    else if (lessonType === 'Transfiguration') {
        return '<img class="card-table-cell-lesson-image" src="/images/lessons/transfiguration.svg" />';
    }
}
function GetRarityImageElementFromRaritySymbol(symbol) {
    if (symbol === 'C') {
        return '<img class="card-table-cell-rarity-image" src="/images/rarities/common.png" />';
    }
    else if (symbol === 'R') {
        return '<img class="card-table-cell-rarity-image" src="/images/rarities/rare.png" />';
    }
    else if (symbol === 'U') {
        return '<img class="card-table-cell-rarity-image" src="/images/rarities/uncommon.png" />';
    }
    else if (symbol === 'FP') {
        return '<img class="card-table-cell-rarity-image" src="/images/rarities/foil-premium.png" />';
    }
    else if (symbol === 'HP') {
        return '<img class="card-table-cell-rarity-image" src="/images/rarities/holo-portrait-premium.png" />';
    }
}

async function SetValuesFromQueryAndPeformCardSearch() {
    var setId = getParameterByName(queryParameterNames.SetId);
    var searchText = getParameterByName(queryParameterNames.SearchText);
    var sortBy = getParameterByName(queryParameterNames.SortBy);
    var sortOrder = getParameterByName(queryParameterNames.SortOrder);
    var cardView = getParameterByName(queryParameterNames.CardView);

    if (searchText) {
        $(searchElementNames.SearchInputId).val(searchText);
    }
    if (sortBy) {
        $(searchElementNames.SortCardsById).val(sortBy);
    }
    if (sortOrder) {
        $(searchElementNames.SortCardsOrderId).val(sortOrder);
    }
    if (cardView) {
        $(searchElementNames.CardViewId).val(cardView);
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
    if (setId || searchText) {
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
        if (searchData.SearchText) {
            queryValues += queryParameterNames.SearchText + '=' + searchData.SearchText + '&';
        }
        if (searchData.SortBy) {
            queryValues += queryParameterNames.SortBy + '=' + searchData.SortBy + '&';
        }
        if (searchData.SortOrder) {
            queryValues += queryParameterNames.SortOrder + '=' + searchData.SortOrder + '&';
        }
        if (searchData.CardView) {
            queryValues += queryParameterNames.CardView + '=' + searchData.CardView + '&';
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
    var searchText = $(searchElementNames.SearchInputId).val().trim();
    var sortBy = $(searchElementNames.SortCardsById).val();
    var sortOrder = $(searchElementNames.SortCardsOrderById).val();
    var cardView = $(searchElementNames.CardViewId).val();

    if (setId === '00000000-0000-0000-0000-000000000000' || setId === '') {
        setId = null;
    }
    //Temporary until I can convene with the others to determine what auto will mean later
    if (sortOrder === 'Auto') {
        sortOrder = 'asc';
    }

    const searchData = {
        SetId: setId,
        SearchText: searchText,
        SortBy: sortBy,
        SortOrder: sortOrder,
        CardView: cardView,
    };

    return searchData;
}

//function SetSearchLoadingState(loading) {
//    if (loading === 'loading') {
//        $('#searchButton').addClass('disabled');
//        $('#searchButtonIcon').addClass('d-none');
//        $('#searchButtonSpinner').removeClass('d-none');
//    }
//    else {
//        $('#searchButton').removeClass('disabled');
//        $('#searchButtonIcon').removeClass('d-none');
//        $('#searchButtonSpinner').addClass('d-none');
//    }
//}

//function ShowCardModal(id) {
//    $(searchElementNames.CardModalId).modal();
//}