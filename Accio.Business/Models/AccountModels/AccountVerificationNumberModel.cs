using System;

namespace Accio.Business.Models.AccountModels
{
    public class AccountVerificationNumberModel
    {
        public Guid AccountVerificationNumberId { get; set; }
        public Guid AccountId { get; set; }
        public int Number { get; set; }
        public DateTime Expires { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
