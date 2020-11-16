using System;
using System.Collections.Generic;

#nullable disable

namespace Accio.Data
{
    public partial class AccountRole
    {
        public Guid AccountRoleId { get; set; }
        public Guid RoleId { get; set; }
        public Guid AccountId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
