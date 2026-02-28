using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public int PolicyRequestId { get; set; }
        public PolicyRequest PolicyRequest { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime GeneratedDate { get; set; }

        public decimal TotalPremium { get; set; }
        public decimal InstallmentAmount { get; set; }
        public int InstallmentCount { get; set; }

        public decimal ClaimAmount { get; set; }

        public string PlanName { get; set; }

        public int CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }
    }
}
