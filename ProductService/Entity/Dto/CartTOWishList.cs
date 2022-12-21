using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Dto
{
    public class CartTOWishList
    {
        public Guid WishListId { get; set; }

        public Guid ProductId { get; set; } 

    }
}
