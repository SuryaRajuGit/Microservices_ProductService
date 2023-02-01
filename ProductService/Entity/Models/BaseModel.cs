using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Models
{
    public class BaseModel
    {
        public Guid Id { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime  CreatedDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool IsActive { get; set; }

    }
}
