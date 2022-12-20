using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class ProductDTO
    {
        public Guid Category_id { get; set; }

        public ICollection<CategoryDTO> Categories { get; set; }

    }
}
