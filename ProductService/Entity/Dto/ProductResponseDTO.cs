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
        [JsonProperty("asset")]
        public byte[] Asset { get; set; }

    }
}
