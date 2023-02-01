﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Models
{
    public class Catalog : BaseModel
    {

        public string Name { get; set; }

        public ICollection<Category> Category { get; set; }
    }
}
