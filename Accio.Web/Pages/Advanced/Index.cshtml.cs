﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accio.Business.Models.SetModels;
using Accio.Business.Models.TypeModels;
using Accio.Business.Services.AdvancedCardSearchSearchServices;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.SourceServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accio.Presentation.Web.Pages.Advanced
{
    public class IndexModel : PageModel
    {
        public List<SetModel> Sets { get; set; }
        public List<CardTypeModel> CardTypes { get; set; }

        private SourceService _sourceService { get; set; }
        private AdvancedCardSearchService _advancedCardSearchService { get; set; }
        private SetService _setService { get; set; }
        private TypeService _typeService { get; set; }

        public IndexModel(SourceService sourceService, AdvancedCardSearchService advancedCardSearchService,
                          SetService setService, TypeService typeService)
        {
            _sourceService = sourceService;
            _advancedCardSearchService = advancedCardSearchService;
            _setService = setService;
            _typeService = typeService;
        }

        public void OnGet()
        {
            Sets = _setService.GetSets();
            CardTypes = _typeService.GetCardTypes();
        }

        public JsonResult OnPostGetAdvancedSearchUrlValueAsync(string cardName, string cardText, string cardTypes, string lessonTypes,
                                                               string power, string sets, string rarity, string flavorText, string artist,
                                                               string cardNumber, string provides)
        {
            try
            {
                var searchUrl = _advancedCardSearchService.GetAdvancedSearchUrlValue(cardName, cardText, cardTypes, lessonTypes,
                                                                                     power, sets, rarity, flavorText, artist,
                                                                                     cardNumber, provides);
                return new JsonResult(new { success = true, json = searchUrl });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
    }
}