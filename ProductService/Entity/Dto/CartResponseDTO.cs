using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class CartResponseDTO
    { 
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("asset")]
        public byte[] Asset { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
