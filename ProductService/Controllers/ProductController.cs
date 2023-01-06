using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Contracts;
using ProductService.Entity.Dto;
using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProductService.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;
  
        public ProductController(IProductServices productServices )
        {
            _productServices = productServices;
        }

        [HttpPost]
        [Route("api/admin/product")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<SaveProductResponse>> AddProduct([FromBody] ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO iscatelogidexists = _productServices.IsCatelogIdexists(product);
            if (iscatelogidexists != null)
            {
                return NotFound(iscatelogidexists);
            }
            ErrorDTO isproductexists = _productServices.IsProductExists(product.Product.Select(item => item.Name));
            if (isproductexists != null)
            {
                return StatusCode(409, isproductexists);
            }
            List < SaveProductResponse > response = _productServices.SaveProduct(product);
            return StatusCode(201, response);
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        [Route("api/product/{id}/category/{category-id}")]
        public ActionResult<ProductResponseDTO> GetProduct([FromRoute(Name = "id")] Guid id, [FromRoute(Name = "category-id")] Guid categoryId )
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isProductExists = _productServices.IsProductExists(id, categoryId);
            if(isProductExists != null)
            {
                return StatusCode(404, isProductExists);
            }
            ProductResponseDTO response = _productServices.GetProduct(id, categoryId);
            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("/api/admin/product")]
        public IActionResult UpdateProductDetails([FromBody] UpdateProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            var isProductNameExists = _productServices.IsProductNameExists(product);
            if(isProductNameExists != null)
            {
                return StatusCode(409, isProductNameExists);
            }
            ErrorDTO updateProduct = _productServices.UpdateProduct(product);
            if (updateProduct != null)
            {
                return NotFound(updateProduct);
            }
            return Ok("Product details updated sucessfully");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/admin")]
        public ActionResult<CatalogResponseDTO> CreateCatalog([FromBody] CatalogDTO catalogDTO)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isCatalogNameExists = _productServices.IsCatalogNameExists(catalogDTO);
            if(isCatalogNameExists != null)
            {
                return StatusCode(409, isCatalogNameExists);
            }
            CatalogResponseDTO saveCatalog = _productServices.SaveCatalog(catalogDTO);
            return StatusCode(201,saveCatalog);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/product/ocelot")]
        public string IsProductExistsInventory([FromQuery]Guid id,[FromQuery] Guid categoryId )
        {
            return _productServices.GetProductQuantity(id, categoryId);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("api/admin/product/{id}")]
        public IActionResult DeleteProduct([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO deleteProduct = _productServices.DeleteProduct(id);
            if(deleteProduct != null)
            {
                return StatusCode(404,deleteProduct);
            }
            return Ok("Product deleted successfully");

        }
        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        [Route("api/product/update")] 
        public IActionResult UpdateProductQuantity([FromBody] ProductToCartDTO updatePurchasedProduct )
        {
            var x = _productServices.UpdatePurchaseProduct(updatePurchasedProduct);
            if(x == false)
            {
                return StatusCode(404);
            }
            return Ok("Product quantity updated successfully");
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/cart/products")]
        public IActionResult IsCartproductsExist([FromBody] List<Guid> ProductIds )
        {
            var d = _productServices.CheckProducts(ProductIds);
            return Ok(d);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/cart/products/qunatity")]
        public IActionResult CartProductsQuantity([FromBody] List<ProductQunatity> productQunatities)
        {

            var f = _productServices.CheckProductQunatity(productQunatities);
            return Ok(f);
            
        }
        [HttpPost]
        [Authorize(Roles ="Admin,User")]
        [Route("api/cart/products/price")]
        public IActionResult CartProductsPrice([FromBody] List<ProductQunatity> products)
        {
            var f = _productServices.CartProductsPrice(products);
            if(f == null)
            {
                return Ok(null);
            }
            return Ok(f);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/products")]
        public IActionResult GetProductList([FromQuery(Name = "category-id")] Guid categoryId, [FromQuery(Name = "catalog-id")] Guid catalogId, [FromQuery] int size=5, [FromQuery(Name ="page-no")]int pageNo=1,
        [FromQuery(Name ="sort-by")] string sortBy="Price",[FromQuery(Name ="sort-order")] string sortOrder="ASC",[FromQuery]string name="")
        {
            ErrorDTO validFields = _productServices.ValidateFields(catalogId,categoryId,name);
            if(validFields != null)
            {
                return BadRequest(validFields);
            }
            ErrorDTO isCategoryIdExist = _productServices.IsCategoryIdExist(catalogId,categoryId,name);
            if(isCategoryIdExist != null)
            {
                return StatusCode(404,isCategoryIdExist);
            }
            List<ProductResponseDTO> products = _productServices.GetProductList(catalogId, categoryId,size,pageNo,sortBy,sortOrder,name);
            if(products == null)
            {
                return StatusCode(204,"No products found");
            }
            return Ok(products);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/product/{id}/price")]
        public float GetProductPrice([FromRoute] Guid id)
        {
            return _productServices.GetProductPrice(id);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/product/details")]
        public List<CartResponseDTO> GetCartProductDetails([FromBody] List<ProductQunatity> products )
        {
            return _productServices.GetCartProductDetails(products);
        }
        [HttpPost]
        [Authorize(Roles ="Admin,User")]
        [Route("/api/wish-list/product/details")]
        public List<CartResponseDTO> GetWishListProductDetails([FromBody] List<Guid> productIds )
        {
            return _productServices.GetWishListProductDetails(productIds);
        }
    }
}
