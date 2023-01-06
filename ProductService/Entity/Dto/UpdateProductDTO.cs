using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class UpdateProductDTO
    {
        [Required]
        [JsonProperty("category_id")]
        public Guid CategoryId { get; set; }

        [Required]
        [JsonProperty("product_id")]
        public Guid ProductId { get; set; }

        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("asset")]
        public byte[] Asset { get; set; }

        [Required]
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [Required]
        [JsonProperty("price")]
        public float Price { get; set; }

        [Required]
        [JsonProperty("visibility")]
        public bool Visibility { get; set; }
    }
}
