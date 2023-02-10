using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class BillProductDTO
    {
        public int BillId { get; set; } 

        public List<Guid> ProductIds { get; set; }
    }
}
