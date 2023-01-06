using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class CatalogResponseDTO
    {
        [JsonProperty(PropertyName = "catalog_id")]
        public Guid CatalogId { get; set; }

        [JsonProperty(PropertyName = "catalog_name")]
        public string CatalogName { get; set; }

        [JsonProperty(PropertyName = "category")]
        public List<CatelogCategoryDTO> Category { get; set; }
    }
}
