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

        [JsonProperty("name")]
        public string? Name { get; set; }

   
        [JsonProperty("description")]
        public string? Description { get; set; }

        [RegularExpression(@"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)?$")]
        [JsonProperty("asset")]
        public string? Asset { get; set; }

        [RegularExpression(@"^[1-9][0-9]*$")]
        [JsonProperty(PropertyName = "price")]
        public float? Price { get; set; }

        [RegularExpression(@"^[0-9]+$")]
        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

    
        [JsonProperty("visibility")]
        public bool? Visibility { get; set; }
    }
}
