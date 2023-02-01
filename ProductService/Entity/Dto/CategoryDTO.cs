using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class CategoryDTO
    {
        [Required]
        [RegularExpression(@"^[1-9][0-9]*$")]
        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        [RegularExpression(@"^[0-9]+$")]
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("visibility")]
        public bool Visibility { get; set; }

       
        [JsonProperty("asset")]
        public byte[] Asset { get; set; }

        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("description")]
        public string Description { get; set; }

    }
}
