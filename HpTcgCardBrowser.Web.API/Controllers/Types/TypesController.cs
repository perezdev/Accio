using System.Collections.Generic;
using HpTcgCardBrowser.Business.Models.TypeModels;
using HpTcgCardBrowser.Business.Services.CardServices;
using Microsoft.AspNetCore.Mvc;

namespace HpTcgCardBrowser.Web.API.Controllers.Types
{
    [Route("Types")]
    [ApiController]
    public class TypesController : ControllerBase
    {
        public TypeService _typeService { get; set; }

        public TypesController(TypeService typeService)
        {
            _typeService = typeService;
        }

        [HttpGet]
        public IEnumerable<CardTypeModel> Get()
        {
            var cardTypes = _typeService.GetCardTypes();
            return cardTypes;
        }
    }
}