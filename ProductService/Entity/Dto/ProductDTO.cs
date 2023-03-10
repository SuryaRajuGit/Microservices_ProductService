using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class ProductDTO
    {
        [Required]
        [JsonProperty("category_id")]
        public Guid CategoryId { get; set; }

        [Required]
        [JsonProperty("product")]
        public ICollection<CategoryDTO> Product { get; set; }

    }
}
