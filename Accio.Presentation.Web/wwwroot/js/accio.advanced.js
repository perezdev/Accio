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
    StatsValueInputId: '#statsValueInput',
    StatsTypeId: '#statsType',
    StatsOperatorId: '#statsOperator',
    StatsIemsContainerId: '#statsIemsContainer',
    StatValueInputClassName: '.stat-value-input',
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

    $(advancedSearchElements.CardNameId + ',' + advancedSearchElements.CardTextId + ',' + advancedSearchElements.PowerId + ',' +
        advancedSearchElements.FlavorTextId + ',' + advancedSearchElements.ArtistId + ',' + advancedSearchElements.CardNumberId + ',' +
        advancedSearchElements.KeywordId + ',' + advancedSearchElements.ComcCheckboxId + ',' + advancedSearchElements.CharmsCheckboxId + ',' +
        advancedSearchElements.PotionsCheckboxId + ',' + advancedSearchElements.QuidditchCheckboxId + ',' + advancedSearchElements.TransfigurationCheckboxId + ',' +
        advancedSearchElements.CommonCheckboxId + ',' + advancedSearchElements.UncommonCheckboxId + ',' + advancedSearchElements.RareCheckboxId + ',' +
        advancedSearchElements.PremiumCheckboxId + ',' + advancedSearchElements.ProvidesComcCheckboxId + ',' + advancedSearchElements.ProvidesCharmsCheckboxId + ',' +
        advancedSearchElements.ProvidesPotionsCheckboxId + ',' + advancedSearchElements.ProvidesQuidditchCheckboxId + ',' + advancedSearchElements.ProvidesTransfigurationCheckboxId
    ).on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            RedirectToSearchWithAdvancedSearchString();
        }
    });

    $('.ui.dropdown').dropdown({
        clearable: true,
    });

    $(advancedSearchElements.StatValueInputClassName).on('keypress', function (e) {
        if (event.which != 8 && isNaN(String.fromCharCode(event.which))) {
            event.preventDefault(); //stop character from entering input
        }
    });

    $(advancedSearchElements.StatsValueInputId).on('keyup', function (e) {
        //Perform search if enter is pressed. Add new item otherwise
        if (e.which === 13) {
            e.preventDefault();
            RedirectToSearchWithAdvancedSearchString();
        }
        else {
            if (!this.value) {
                return;
            }

            var type = $(advancedSearchElements.StatsTypeId).val();
            var operator = $(advancedSearchElements.StatsOperatorId).val();
            var value = $(advancedSearchElements.StatsValueInputId).val();

            AddStatsItem(type, operator, value);

            //Reset default settings
            $(advancedSearchElements.StatsTypeId).prop('selectedIndex', 0).change();
            $(advancedSearchElements.StatsOperatorId).prop('selectedIndex', 0).change();
            $(advancedSearchElements.StatsValueInputId).val('');

            //Select the last input in the main container and set the cursor at the end so the user can keep typing if needed
            var input = $(".stat-value-input").last();
            input.select();
            input[0].selectionStart = input[0].selectionEnd = input.val().length;
        }
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
    var statItems = GetStatItems();

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
    if (statItems) {
        fd.append('stats', statItems);
    }

    $.ajax({
        type: "POST",
        url: "advanced?handler=GetAdvancedSearchUrlValue",
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
                var cardRoute = '/search?searchText=' + url;
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
function AddStatsItem(type, operator, value) {
    var healthOption = '<option>Health</option>';
    var damageOption = '<option>Damage</option>';

    if (type === 'Health') {
        healthOption = '<option selected="selected">Health</option>';
    } else if (type === 'Damage') {
        healthOption = '<option selected="selected">Damage</option>';
    }

    var greaterThanOption = '<option>greater than</option>'
    var greaterThanOrEqualToOption = '<option>greater than or equal to</option>'
    var lessThanOption = '<option>less than</option>'
    var lessThanOrEqualToOption = '<option>less than or equal to</option>'
    var equalToOption = '<option>equal to</option>'

    if (operator === 'greater than') {
        greaterThanOption = '<option selected="selected">greater than</option>';
    }
    else if (operator === 'greater than or equal to') {
        greaterThanOrEqualToOption = '<option selected="selected">greater than or equal to</option>';
    }
    else if (operator === 'less than') {
        lessThanOption = '<option selected="selected">less than</option>';
    }
    else if (operator === 'less than or equal to') {
        lessThanOrEqualToOption = '<option selected="selected">less than or equal to</option>';
    }
    else if (operator === 'equal to') {
        equalToOption = '<option selected="selected">equal to</option>';
    }

    var valueInput = '<input class="stat-value-input input-reset pl2 ml2" type="text" value="' + value + '" />';

    //We create a random ID per stat item so we can identify the container for each item in order to
    //remove it when needed. The same ID is used for the image to setup the click event for removal
    var rid = Math.floor(Math.random() * 1000000000);
    var removeItemId = 'remove' + rid;

    var statsItem = `
        <div id="` + rid + `" class="flex flex-items-start mt2">
           <div class="fl ml3">
              <select class="select-stats">
                 ` + healthOption + `
                 ` + damageOption + `
              </select>
           </div>
           <div class="fl ml2">
               <select class="select-stats">
                    ` + greaterThanOption + `
                    ` + greaterThanOrEqualToOption + `
                    ` + lessThanOption + `
                    ` + lessThanOrEqualToOption + `
                    ` + equalToOption + `
               </select>
           </div>
           <div class="fl">
               ` + valueInput + `
           </div>
           <div class="fl">
               <div id="` + removeItemId + `" class="ml2 stat-item-delete-img"></div>
           </div>
        </div>
    `;

    $(advancedSearchElements.StatsIemsContainerId).append(statsItem);

    $('#' + removeItemId).click(function (e) {
        //Remove container of stat item
        $('#' + rid).remove();
    });
}
function GetStatItems() {
    var items = '';

    //Loop through each div in the main container. Each div is the container for each item's type, operator, and value.
    $(advancedSearchElements.StatsIemsContainerId + ' >  div').each(function (i) {
        $(this).each(function (ii) {
            var type = $(this).find('select')[0];
            var operator = $(this).find('select')[1];
            var value = $(this).find('input')[0];

            var typeText = $(type).find(':selected').text();
            var operatorText = $(operator).find(':selected').text();
            var valueText = $(value).val();

            if (items.includes(typeText)) {
                return true;
            }

            var item = typeText + '|' + operatorText + '|' + valueText;
            items += item + ';'
        });
    });

    //String of items where each item is separated by a semicolon and each value in each item is separated by a comma
    return items;
}