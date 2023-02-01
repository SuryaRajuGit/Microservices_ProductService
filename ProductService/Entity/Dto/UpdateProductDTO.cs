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
        [JsonProperty("id")]
        public Guid Id { get; set; }


        [JsonProperty("name")]
        public string Name { get; set; }

   
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("asset")]
        public byte[] Asset { get; set; }

        [RegularExpression(@"^[1-9][0-9]*$")]
        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        [RegularExpression(@"^[0-9]+$")]
        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

    
        [JsonProperty("visibility")]
        public bool? Visibility { get; set; }
    }
}
