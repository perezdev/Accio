using System.Collections.Generic;
using HpTcgCardBrowser.Business.Models.RarityModels;
using HpTcgCardBrowser.Business.Services.CardServices;
using Microsoft.AspNetCore.Mvc;

namespace HpTcgCardBrowser.Web.API.Controllers.Rarities
{
    [Route("Rarities")]
    [ApiController]
    public class RaritiesController : ControllerBase
    {
        public RarityService _rarityService { get; set; }

        public RaritiesController(RarityService rarityService)
        {
            _rarityService = rarityService;
        }

        [HttpGet]
        public IEnumerable<RarityModel> Get()
        {
            var rarities = _rarityService.GetCardRarities();
            return rarities;
        }
    }
}