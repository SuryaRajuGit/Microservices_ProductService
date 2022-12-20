using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class UpdateProductDTO
    {
        public Guid CategoryId { get; set; }

        public Guid ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public float Price { get; set; }

        public bool Visibility { get; set; }
    }
}
