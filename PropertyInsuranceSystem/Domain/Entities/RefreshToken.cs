using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public bool IsRevoked { get; set; } = false;

        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
