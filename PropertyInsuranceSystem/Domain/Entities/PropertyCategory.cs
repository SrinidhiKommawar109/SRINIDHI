using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PropertyCategory :BaseEntity
    {
        public string Name { get; set; }
        public ICollection<PropertySubCategory> SubCategories { get; set; }
    }
}
