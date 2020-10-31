const setsPageElements = {
    SetsTableId: '#setsTable',
};
const SetsTableColumnIndex = {
    SetId: 0,
    Name: 1,
    TotalCards: 2,
    ReleaseDate: 3,
    Languages: 4,
};

function InitializeSetsPage() {
    /* Sets table initialization */
    InitializeSetsTable();

    /* Populate sets table */
    PopulateSetsTable();

    /* Card search initialization */
    InitializeSearchBoxOnNonSearchPage();
}

var setsTable = null;
function InitializeSetsTable() {
    setsTable = $(setsPageElements.SetsTableId).DataTable({
        lengthChange: false,
        searching: false,
        paging: false,
        bInfo: false,
        columnDefs: [
            {
                //Hide the card ID column
                targets: [0],
                visible: false,
            },
            {
                //Align the name to the left of the column. The 100% class is used to extend the text
                //so row over works correctly
                targets: [1],
                className: 'fl w-100'
            },
            {
                //Cap the width of the cards and date column
                targets: [2, 3],
                width: '10%'
            },
            {
                //Align the content of the language column vertically centered
                targets: [4],
                className: 'v-mid',
            }
        ]
    });

    $(setsPageElements.SetsTableId + ' tbody').on('click', 'tr', function () {
        var data = setsTable.row(this).data();
        window.location.href = '/Search?set=' + data[0] + '&sortBy=sn&cardView=images';
    });
}

function PopulateSetsTable() {
    $.ajax({
        type: "POST",
        url: "Sets?handler=GetSets",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                var sets = response.json;
                AddSetsToSetsTable(sets);
            }
        },
        failure: function (response) {
            alert('Catastropic error');
        }
    });
}
function AddSetsToSetsTable(sets) {
    for (var i = 0; i < sets.length; i++) {
        var set = sets[i];

        var shortNameColumn = set.shortName.toLowerCase();
        var nameColumn = '<div class="flex items-center"><img class="sets-set-table-set-icon" src="/images/seticons/' + set.iconFileName + '" /><div>' + set.name + '</div></div>';
        var totalCardsColumn = set.totalCards;
        var releaseDateColumn = set.releaseDate;

        var languages = '<div class="flex items-center">';
        for (var n = 0; n < set.languages.length; n++) {
            var language = set.languages[n];
            var className = '';
            if (language.enabled) {
                className = 'set-language-enabled';
            }
            else {
                className = 'set-language-disabled';
            }

            languages += '<div class="' + className + '">' + language.code + '</div>';
        }
        languages += '</div>';

        var languageColumn = languages;

        //Add row to table. Passing in a comma separated list for each column will add the columns in that order.
        //The second column is hidden by the column definitions when the table was instantiated
        var rowNode = setsTable.row.add([
            shortNameColumn, nameColumn, totalCardsColumn, releaseDateColumn, languageColumn
        ]);

        setsTable.order([SetsTableColumnIndex.ReleaseDate, 'desc']).draw();
    }
}