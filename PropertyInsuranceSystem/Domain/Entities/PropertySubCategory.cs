using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PropertySubCategory : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }

        public PropertyCategory Category { get; set; }

        public ICollection<PropertyPlans> Plans { get; set; }
    }
}
