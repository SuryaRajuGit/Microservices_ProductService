using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class WishListToCartDTO
    {
        [Required]
        [JsonProperty("wishlist_id")]
        public Guid WishListId { get; set; }

        [Required]
        [JsonProperty("product_id")]
        public Guid ProductId { get; set; } 

    }
}
