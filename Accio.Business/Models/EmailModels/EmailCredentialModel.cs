using System.Net.Mail;

namespace Accio.Business.Models.EmailModels
{
    public class EmailCredentialModel
    {
        public string Name { get; set; }
        public MailAddress EmailAddress { get; set; }
        public string SendGridApiKey { get; set; }
    }
}
