using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
namespace Domain.Enums
{
    public enum ClaimStatus
    {
        Pending,       // Claim submitted by customer
        Verified,      // Verified by Claims Officer
        Approved,      // Accepted
        Rejected       // Rejected
    }
}
