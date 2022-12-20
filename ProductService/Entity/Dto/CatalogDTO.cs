using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class CatalogDTO
    {
        public string CatalogName { get; set; }

        public ICollection<CategoryDTOcatalog>  Category { get; set; }
    }
}
