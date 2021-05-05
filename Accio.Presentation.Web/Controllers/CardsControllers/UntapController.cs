using Accio.Business.Models.ApiModels.UntapModels;
using Accio.Business.Services.ApiServices.UntapServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Accio.Presentation.Web.Controllers.CardsControllers
{
    [Route("api/cards/untap")]
    public class UntapController : BaseController
    {
        private UntapService _untapService { get; set; }

        public UntapController(UntapService untapService)
        {
            _untapService = untapService;
        }

        [HttpGet]
        public IEnumerable<UntapCardModel> Get()
        {
            var cards = _untapService.GetAllCards();
            return cards;
        }
    }
}
