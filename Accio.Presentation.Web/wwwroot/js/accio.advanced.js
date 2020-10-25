const advancedSearchElements = {
    CardNameId: '#cardName',
    CardTextId: '#cardText',
    SetsDropDownId: '#sets',
    CardTypesDropDownId: '#cardTypes',
    PowerId: '#power',
    ComcCheckboxId: '#comcCheckbox',
    CharmsCheckboxId: '#charmsCheckbox',
    PotionsCheckboxId: '#potionsCheckbox',
    QuidditchCheckboxId: '#quidditchCheckbox',
    TransfigurationCheckboxId: '#transfigurationCheckbox',
    ProvidesComcCheckboxId: '#providesComcCheckbox',
    ProvidesCharmsCheckboxId: '#providesCharmsCheckbox',
    ProvidesPotionsCheckboxId: '#providesPotionsCheckbox',
    ProvidesQuidditchCheckboxId: '#providesQuidditchCheckbox',
    ProvidesTransfigurationCheckboxId: '#providesTransfigurationCheckbox',
    CommonCheckboxId: '#commonCheckbox',
    UncommonCheckboxId: '#uncommonCheckbox',
    RareCheckboxId: '#rareCheckbox',
    PremiumCheckboxId: '#premiumCheckbox',
    FlavorTextId: '#flavorText',
    ArtistId: '#artist',
    CardNumberId: '#cardNumber',
    KeywordId: '#keyword',
};

function InitializeAdvancedPage() {
    /* Advanced search initialization */
    InitializeAdvancedSearchElements();

    /* Card search initialization */
    InitializeSearchBoxOnNonSearchPage();
}

function InitializeAdvancedSearchElements() {
    $('.search-button').click(function (e) {
        e.preventDefault();
        RedirectToSearchWithAdvancedSearchString();
    });

    $(advancedSearchElements.CardNameId + ',' + advancedSearchElements.CardTextId +
        ',' + advancedSearchElements.PowerId + ',' + advancedSearchElements.FlavorTextId +
        ',' + advancedSearchElements.ArtistId, + ',' + advancedSearchElements.CardNumberId).on('keypress', function (e) {
            if (e.which === 13) {
                e.preventDefault();
                RedirectToSearchWithAdvancedSearchString();
            }
        });

    $('.ui.dropdown').dropdown({
        clearable: true,
    });
}

function RedirectToSearchWithAdvancedSearchString() {
    var cardName = $(advancedSearchElements.CardNameId).val();
    var cardText = $(advancedSearchElements.CardTextId).val();
    var cardTypes = $(advancedSearchElements.CardTypesDropDownId).dropdown('get value');
    var lessonTypes = GetLessonsDelimitedString('lessonTypes');
    var power = $(advancedSearchElements.PowerId).val();
    var sets = $(advancedSearchElements.SetsDropDownId).dropdown('get value');
    var rarity = GetRarityDelimitedString();
    var flavorText = $(advancedSearchElements.FlavorTextId).val();
    var artist = $(advancedSearchElements.ArtistId).val();
    var cardNumber = $(advancedSearchElements.CardNumberId).val();
    var provides = GetLessonsDelimitedString('provides');
    var keyword = $(advancedSearchElements.KeywordId).val();

    var fd = new FormData();
    if (cardName) {
        fd.append('cardName', cardName);
    }
    if (cardText) {
        fd.append('cardText', cardText);
    }
    if (cardTypes) {
        fd.append('cardTypes', cardTypes);
    }
    if (lessonTypes) {
        fd.append('lessonTypes', lessonTypes);
    }
    if (power) {
        fd.append('power', power);
    }
    if (sets) {
        fd.append('sets', sets);
    }
    if (rarity) {
        fd.append('rarity', rarity);
    }
    if (flavorText) {
        fd.append('flavorText', flavorText);
    }
    if (artist) {
        fd.append('artist', artist);
    }
    if (cardNumber) {
        fd.append('cardNumber', cardNumber);
    }
    if (provides) {
        fd.append('provides', provides);
    }
    if (keyword) {
        fd.append('keyword', keyword);
    }

    $.ajax({
        type: "POST",
        url: "Advanced?handler=GetAdvancedSearchUrlValue",
        traditional: true,
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
                var cardRoute = '/Search?searchText=' + url;
                window.location.href = baseUrl + cardRoute;
            }
        },
        failure: function (response) {
            alert('Catastropic error');
        }
    });
}
function GetLessonsDelimitedString(type) {
    var val = '';
    if (type === 'lessonTypes') {
        var lessonComc = $(advancedSearchElements.ComcCheckboxId).is(':checked') ? 'comc,' : null;
        var lessonCharms = $(advancedSearchElements.CharmsCheckboxId).is(':checked') ? 'c,' : null;
        var lessonPotions = $(advancedSearchElements.PotionsCheckboxId).is(':checked') ? 'p,' : null;
        var lessonQuidditch = $(advancedSearchElements.QuidditchCheckboxId).is(':checked') ? 'q,' : null;
        var lessonTransfiguration = $(advancedSearchElements.TransfigurationCheckboxId).is(':checked') ? 't,' : null;

        if (lessonComc) {
            val += lessonComc;
        }
        if (lessonCharms) {
            val += lessonCharms;
        }
        if (lessonPotions) {
            val += lessonPotions;
        }
        if (lessonQuidditch) {
            val += lessonQuidditch;
        }
        if (lessonTransfiguration) {
            val += lessonTransfiguration;
        }
    } else if (type === 'provides') {
        var providesComc = $(advancedSearchElements.ProvidesComcCheckboxId).is(':checked') ? 'comc,' : null;
        var providesCharms = $(advancedSearchElements.ProvidesCharmsCheckboxId).is(':checked') ? 'c,' : null;
        var providesPotions = $(advancedSearchElements.ProvidesPotionsCheckboxId).is(':checked') ? 'p,' : null;
        var providesQuidditch = $(advancedSearchElements.ProvidesQuidditchCheckboxId).is(':checked') ? 'q,' : null;
        var providesTransfiguration = $(advancedSearchElements.ProvidesTransfigurationCheckboxId).is(':checked') ? 't,' : null;

        if (providesComc) {
            val += providesComc;
        }
        if (providesCharms) {
            val += providesCharms;
        }
        if (providesPotions) {
            val += providesPotions;
        }
        if (providesQuidditch) {
            val += providesQuidditch;
        }
        if (providesTransfiguration) {
            val += providesTransfiguration;
        }
    }

    return val.slice(0, -1).trim();
}
function GetRarityDelimitedString() {
    var val = '';

    var commonRarity = $(advancedSearchElements.CommonCheckboxId).is(':checked') ? 'c,' : null;
    var uncommonRarity = $(advancedSearchElements.UncommonCheckboxId).is(':checked') ? 'u,' : null;
    var rareRarity = $(advancedSearchElements.RareCheckboxId).is(':checked') ? 'r,' : null;
    var premiumRarity = $(advancedSearchElements.PremiumCheckboxId).is(':checked') ? 'r,' : null;

    if (commonRarity) {
        val += commonRarity;
    }
    if (uncommonRarity) {
        val += uncommonRarity;
    }
    if (rareRarity) {
        val += rareRarity;
    }
    if (premiumRarity) {
        val += premiumRarity;
    }

    return val.slice(0, -1).trim();
}