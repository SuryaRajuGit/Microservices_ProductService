using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Models
{
    public class Product : BaseModel
    {

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        public float Price { get; set; }

        public int Quantity { get; set; }

        public bool Visibility { get; set; }

        public string Asset { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

    }
}
