using System.Collections.Generic;
using HpTcgCardBrowser.Business.Models.SetModels;
using HpTcgCardBrowser.Business.Services.CardServices;
using Microsoft.AspNetCore.Mvc;

namespace HpTcgCardBrowser.Web.API.Controllers.Sets
{
    [Route("Sets")]
    [ApiController]
    public class SetsController : ControllerBase
    {

        private SetService _setService { get; set; }

        public SetsController(SetService setService)
        {
            _setService = setService;
        }

        [HttpGet]
        public IEnumerable<SetModel> Get()
        {
            var sets = _setService.GetSets();
            return sets;
        }
    }
}