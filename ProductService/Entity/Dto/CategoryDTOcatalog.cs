using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class CategoryDTOcatalog
    {
        public string Name { get; set; }

        public ICollection<CategoryDTO> Products { get; set; }
    }
}
