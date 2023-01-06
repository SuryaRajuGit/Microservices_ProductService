using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class ProductToCartDTO
    {
        [Required]
        [JsonProperty("product_id")]
        public Guid ProductId { get; set; }

        [Required]
        [JsonProperty("category_id")]
        public Guid CategoryId { get; set; }

        [Required]
        [JsonProperty("quantity")]
        public int Quantity { get; set; }


    }
}
