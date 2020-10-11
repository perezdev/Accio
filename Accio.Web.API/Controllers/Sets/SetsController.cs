using System.Collections.Generic;
using Accio.Business.Models.SetModels;
using Accio.Business.Services.CardServices;
using Microsoft.AspNetCore.Mvc;

namespace Accio.Presentation.Web.API.Controllers.Sets
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