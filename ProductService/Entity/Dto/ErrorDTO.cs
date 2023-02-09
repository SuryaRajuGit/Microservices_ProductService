using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class ErrorDTO
    {
        public string type { get; set; }

        public string message { get; set; }

        public string statusCode { get; set; }
    }
}
