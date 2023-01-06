using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class CatalogDTO
    {
        [Required]
        [JsonProperty("catalog_name")]
        public string CatalogName { get; set; }

        [Required]
        [JsonProperty("catalog")]
        public ICollection<CategoryDTOcatalog>  Category { get; set; }
    }
}
