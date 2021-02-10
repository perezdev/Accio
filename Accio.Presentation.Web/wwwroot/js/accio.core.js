//Eventually, I need to split all this code into several files and then bundle on build
var currentPage = null;

const Page = {
    Home: '',
    Search: 'search',
    Card: 'card',
    Sets: 'sets',
    Advanced: 'advanced',
    DeckBuilder: 'deckbuilder',
};

var IsMobile = false; //initiate as false
// device detection
if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|ipad|iris|kindle|Android|Silk|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(navigator.userAgent)
    || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(navigator.userAgent.substr(0, 4))) {
    IsMobile = true;
}

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
    else if (currentPage === Page.DeckBuilder) {
        InitializeDeckBuilderPage();
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
            window.location.href = '/search?searchText=' + $(this).val() + '&sortBy=sn&cardView=images';
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