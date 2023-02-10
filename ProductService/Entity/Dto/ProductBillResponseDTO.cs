using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class ProductBillResponseDTO
    {
        public int BillId { get; set; }

        public List<ProductDetailsDto> Products { get; set; }
    }
}
