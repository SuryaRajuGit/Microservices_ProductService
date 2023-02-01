using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class GetCatalogResponseDTO
    {
        [JsonProperty(PropertyName = "category_id")]
        public Guid CategoryId { get; set; }

        [JsonProperty(PropertyName = "category_name")]
        public string CategoryName { get; set; }

        [JsonProperty(PropertyName = "product")]
        public List<ProductResponseDTO> Product { get; set; }

    }
}
