using System.Collections.Generic;

namespace Accio.Business.Models.AccountModels
{
    public class VerifyAccountResult
    {
        public bool Result { get; set; } = true;
        public List<string> Messages { get; set; } = new List<string>();
    }
}
