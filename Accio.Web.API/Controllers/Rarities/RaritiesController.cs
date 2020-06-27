using System.Collections.Generic;
using Accio.Business.Models.RarityModels;
using Accio.Business.Services.CardServices;
using Microsoft.AspNetCore.Mvc;

namespace Accio.Web.API.Controllers.Rarities
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