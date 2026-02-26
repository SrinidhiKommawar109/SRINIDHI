using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PlanResponseDto
    {
        public int Id { get; set; }
        public string PlanName { get; set; }
        public decimal BaseCoverageAmount { get; set; }
        public decimal CoverageRate { get; set; }
    }
}
