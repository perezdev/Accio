using System;
using System.Collections.Generic;

#nullable disable

namespace Accio.Data
{
    public partial class AuthenticationHistory
    {
        public Guid AuthenticationHistoryId { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string BogusData { get; set; }
        public string Type { get; set; }
        public Guid? ClientId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
