using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class ProductPrice
    {
        public Guid Id { get; set; }

        public float Price { get; set; }
    }
}
