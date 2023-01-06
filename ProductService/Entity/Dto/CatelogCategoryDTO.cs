using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class CatelogCategoryDTO
    {
        [JsonProperty("category_id")]
        public Guid CategoryId { get; set; }

        [JsonProperty("category_name")]
        public string CategoryName { get; set; }

        [JsonProperty("product")]
        public List<CatalogProductResponseDTO> Product { get; set; }
    }
}
