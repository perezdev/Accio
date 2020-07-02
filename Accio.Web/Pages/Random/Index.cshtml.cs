using Accio.Business.Services.CardServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accio.Web.Pages.Random
{
    public class IndexModel : PageModel
    {

        private CardService _cardService { get; set; }

        public IndexModel(CardService cardService)
        {
            _cardService = cardService;
        }

        public IActionResult OnGet()
        {
            //Since we're going to redirect to the details page, it makes sense to just grab the ID instead of the full card object
            var randomCardId = _cardService.GetRandomCardId();
            return Redirect($"/Card?cardId={randomCardId}");
        }
    }
}