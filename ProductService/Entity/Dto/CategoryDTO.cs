using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class CategoryDTO
    {
        public float Price { get; set; }

        public int Quantity { get; set; }

        public bool Visibility { get; set; }

        public byte[] Asset { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

    }
}
