using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateClaimDto
    {
        public int PolicyRequestId { get; set; }

        public string PropertyAddress { get; set; }
        public decimal PropertyValue { get; set; }
        public int PropertyAge { get; set; }

      
    }
}
