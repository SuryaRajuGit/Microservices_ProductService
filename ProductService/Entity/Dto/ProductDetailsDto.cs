using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class ProductDetailsDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Asset { get; set; }

        public string Description { get; set; }
    }
}
