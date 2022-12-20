using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Models
{
    public class Category
    {
        public Guid Id { get; set; }

        public Guid CatalogId { get; set; }
        public Catalog Catalog { get; set; }

        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }


    }
}
