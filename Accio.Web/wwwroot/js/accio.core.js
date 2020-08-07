//Eventually, I need to split all this code into several files and then bundle on build
var currentPage = null;

const Page = {
    Home: '',
    Search: 'Search',
    Card: 'Card',
    Sets: 'Sets',
    Advanced: 'Advanced',
};

/**
 * On page load
 */
$(document).ready(function () {
    InitializeLayout();

    //The Search and Card page both derive from the same layout page
    //So they share the same JS. This ensures we only load the stuff for the appropriate page,
    //regardless of the domain.
    currentPage = GetCurrentPage();
    if (currentPage === Page.Home) {
        InitializeHomePage();
    }
    else if (currentPage === Page.Search) {
        InitializeSearchPage();
    }
    else if (currentPage === Page.Card) {
        InitializeCardPage();
    } else if (currentPage === Page.Sets) {
        InitializeSetsPage();
    }
    else if (currentPage === Page.Advanced) {
        InitializeAdvancedPage();
    }
});


function InitializeLayout() {
    InitializeLayouElements();
}

/**
 * Layout
 * -----------------------------------------------------------------------------------------------------
 */
function InitializeLayouElements() {
    //Clear search
    $(searchElementNames.SearchInputId).on('keyup', function () {
        var clear = $(searchElementNames.ClearSearchClassName);
        if ($(this).val() === '') {
            if (!clear.hasClass('vh')) {
                clear.addClass('vh');
            }
        }
        else {
            if (clear.hasClass('vh')) {
                clear.removeClass('vh');
            }
        }
    });
    $(searchElementNames.ClearSearchClassName).on('click', function () {
        var search = $(searchElementNames.SearchInputId);
        search.val('');

        $(this).addClass('vh');
    });
}


function GetCurrentPage() {
    return window.location.pathname.split('/')[1];
}

//The search box will behave differently on the pages that aren't the search page. We'll basically just redirect to the search page
//so that'll seem like a seamless integration
function InitializeSearchBoxOnNonSearchPage() {
    //Search text press enter
    $(searchElementNames.SearchInputId).on('keypress', function (e) {
        if (e.which === 13) {
            window.location.href = '/Search?searchText=' + $(this).val() + '&sortBy=sn&cardView=images';
            e.preventDefault();
        }
    });
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}
function until(conditionFunction) {
    const poll = resolve => {
        if (conditionFunction()) resolve();
        else setTimeout(_ => poll(resolve), 400);
    };
    return new Promise(poll);
}