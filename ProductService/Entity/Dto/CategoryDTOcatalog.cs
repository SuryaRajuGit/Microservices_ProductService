using Newtonsoft.Json;
using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class CategoryDTOcatalog
    {
        [Required]
        [JsonProperty(PropertyName = "category_name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty(PropertyName = "products")]
        public ICollection<CategoryDTO> Products { get; set; }
    }
}
