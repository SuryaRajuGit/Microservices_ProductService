using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class ProductResponseDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("description")]
        public string Description { get; set; }

        [Required]
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [Required]
        [JsonProperty("price")]
        public float Price { get; set; }

        [Required]
        [RegularExpression(@"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)?$")]
        [JsonProperty("asset")]
        public string? Asset { get; set; }

    }
}
