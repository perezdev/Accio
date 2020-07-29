using Accio.Business.Models.CardModels;
using Accio.Business.Services.CardServices;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Accio.Web.Pages
{
    public class IndexModel : PageModel
    {
        public List<CardModel> RandomCards { get; set; }

        private CardService _cardService { get; set; }

        public IndexModel(CardService cardService)
        {
            _cardService = cardService;
        }

        public void OnGet()
        {
            RandomCards = _cardService.GetRandomCards(3);
        }
    }
}
