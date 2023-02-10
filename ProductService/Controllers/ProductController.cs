using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductService.Contracts;
using ProductService.Entity.Dto;
using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;

namespace ProductService.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.Bearer)]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;
        private readonly ILogger _logger;
        public ProductController(IProductServices productServices , ILogger logger)
        {
            _productServices = productServices;
            _logger = logger;
        }

        ///<summary>
        /// Gets all the categories and products form the catalog
        ///</summary>
        ///<return>List<GetCatalogResponseDTO></return>
        [HttpGet] 
        [Route("api/catalog/{id}")] 
        [Authorize(Roles =("Admin,User"))] 
        public ActionResult GetCatalog([FromRoute] Guid id)
        {
            _logger.LogInformation("Get catalog started");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            List<GetCatalogResponseDTO> response = _productServices.GetCatalog(id);
            if(response == null)
            {
                _logger.LogError("Catalog id not found");
                return StatusCode(404, new ErrorDTO() {type="Catalog",message ="Catalog id not found",statusCode="404" });
            }
            return Ok(response);
        }

        ///<summary>
        /// Adds new product to inventory
        ///</summary>
        ///<return>List < SaveProductResponse ></return>
        [HttpPost]
        [Route("api/admin/product")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<SaveProductResponse>> AddProduct([FromBody] ProductDTO product)
        {
            _logger.LogInformation("Add product to inventory started");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isCategoryIdExists = _productServices.IsCategoryIdExists(product);
            if (isCategoryIdExists != null)
            {
                _logger.LogError("Category id not found");
                return NotFound(isCategoryIdExists);
            }
            ErrorDTO isproductexists = _productServices.IsProductExists(product.Product.Select(item => item.Name));
            if (isproductexists != null)
            {
                return StatusCode(409, isproductexists);
            }
            _logger.LogInformation("Product added to inventory successfully");
            _productServices.SaveProduct(product);
            return StatusCode(201,"Product added to inventory successfully");
        }

        ///<summary>
        /// Gets single product details
        ///</summary>
        ///<return>ProductResponseDTO</return>
        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        [Route("api/product/{id}")]
        public ActionResult<ProductResponseDTO> GetProduct([FromRoute(Name = "id")] Guid id)
        {
            _logger.LogInformation("Get single product started");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isProductExists = _productServices.IsProductExists(id);
            if(isProductExists != null)
            {
                _logger.LogError("Product id not found");
                return StatusCode(404, isProductExists);
            }
            _logger.LogInformation("Product retrievd successfully");
            ProductResponseDTO response = _productServices.GetProduct(id);
            return Ok(response);
        }

        ///<summary>
        /// Updates product details in the inventory
        ///</summary>
        ///<return>Product details updated sucessfully</return>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("/api/admin/product")]
        public IActionResult UpdateProductDetails([FromBody] UpdateProductDTO product,[FromRoute] Guid id)
        {

            _logger.LogInformation("Update product details started");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
           
            ErrorDTO isProductNameExists = _productServices.IsProductNameExists(product,id);
            if(isProductNameExists != null)
            {
                return StatusCode(409, isProductNameExists);
            }

            ErrorDTO updateProduct = _productServices.UpdateProduct(product,id);
            if (updateProduct != null)
            {
                _logger.LogError("Product id not found");
                return NotFound(updateProduct);
            }
            _logger.LogInformation("Product details updated sucessfully");
            return Ok("Product details updated sucessfully");
        }

        ///<summary>
        /// Creates new Catalog in the inventory
        ///</summary>
        ///<return>Guid</return>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/admin")]
        public ActionResult<CatalogResponseDTO> CreateCatalog([FromBody] CatalogDTO catalogDTO)
        {
            _logger.LogInformation("Create catalog started");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isQuantity = _productServices.IsQuantityNotNull(catalogDTO);
            if(isQuantity != null)
            {
                return StatusCode(400, new ErrorDTO() {type="BadRequest",message="Product quantity required",statusCode="400" });
            }
            ErrorDTO isCatalogNameExists = _productServices.IsCatalogNameExists(catalogDTO);
            if(isCatalogNameExists != null)
            {
                return StatusCode(409, isCatalogNameExists);
            }
            _logger.LogInformation("Created catalog successfully");
            Guid saveCatalog = _productServices.SaveCatalog(catalogDTO);
            return StatusCode(201,saveCatalog);
        }

        ///<summary>
        /// returns product details
        ///</summary>
        ///<return>string</return>
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/product/ocelot")]
        public string IsProductExistsInventory([FromQuery]Guid id)
        {
            _logger.LogInformation("IsProductExistsInventory started");
            return _productServices.GetProductQuantity(id);
        }

        ///<summary>
        /// Deletes product in the inventory
        ///</summary>
        ///<return>Product deleted successfully</return>
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("api/admin/product/{id}")]
        public IActionResult DeleteProduct([FromRoute] Guid id)
        {
            _logger.LogInformation("Delete Product started");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Entered wrong data");
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO deleteProduct = _productServices.DeleteProduct(id);
            if(deleteProduct != null)
            {
                _logger.LogError("Product id not found");
                return StatusCode(404,deleteProduct);
            }
            _logger.LogInformation("Product deleted successfully");
            return Ok("Product deleted successfully");

        }

        ///<summary>
        /// Updates product quantity 
        ///</summary>
        ///<return>Product quantity updated successfully</return>
        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        [Route("api/product/update")] 
        public IActionResult UpdateProductQuantity([FromBody] ProductToCartDTO updatePurchasedProduct )
        {
            _logger.LogInformation("Update Product quantity started");
            bool isProductIdExist = _productServices.UpdatePurchaseProduct(updatePurchasedProduct);
            if(isProductIdExist == false)
            {
                _logger.LogError("Product id not found");
                return StatusCode(404);
            }
            _logger.LogInformation("Product quantity updated successfully");
            return Ok("Product quantity updated successfully");
        }

        ///<summary>
        /// Checks product details
        ///</summary>
        ///<return>return product details </return>
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/cart/products")]
        public IActionResult IsCartproductsExist([FromBody] List<Guid> ProductIds )
        {
            _logger.LogInformation("IsCartproductsExist started");
            string productDetails = _productServices.CheckProducts(ProductIds);
            return Ok(productDetails);
        }

        ///<summary>
        /// Checks product details
        ///</summary>
        ///<return>return product details </return>
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/cart/products/qunatity")]
        public IActionResult CartProductsQuantity([FromBody] List<ProductQunatity> productQunatities)
        {
            _logger.LogInformation("CartProductsQuantity started");
            string productDetails = _productServices.CheckProductQunatity(productQunatities);
            return Ok(productDetails);
        }

        ///<summary>
        /// Checks product details
        ///</summary>
        ///<return>return product details </return>
        [HttpPost]
        [Authorize(Roles ="Admin,User")]
        [Route("api/cart/products/price")]
        public IActionResult CartProductsPrice([FromBody] List<ProductQunatity> products)
        {
            _logger.LogInformation("CartProductsPrice started");
            string productDetails = _productServices.CartProductsPrice(products);
            if(productDetails == null)
            {
                return Ok(null);
            }
            _logger.LogInformation("Details of all products in cart returned");
            return Ok(productDetails);
        }

        ///<summary>
        /// Checks product details
        ///</summary>
        ///<return>List<ProductResponseDTO> </return>
        
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/products")]
        public IActionResult GetProductList([FromQuery(Name = "category-id")] Guid categoryId, [FromQuery(Name = "catalog-id")] Guid catalogId, [FromQuery] int size=5, [FromQuery(Name ="page-no")]int pageNo=1,
        [FromQuery(Name ="sort-by")] string sortBy="Price",[FromQuery(Name ="sort-order")] string sortOrder="ASC",[FromQuery]string name="")
        {
            _logger.LogInformation("Getting all products in the list strated started");
            ErrorDTO validFields = _productServices.ValidateFields(catalogId,categoryId,name);
            if(validFields != null)
            {
                _logger.LogError("Entered wrong data");
                return BadRequest(validFields);
            }
            ErrorDTO isCategoryIdExist = _productServices.IsCategoryIdExist(catalogId,categoryId,name);
            if(isCategoryIdExist != null)
            {
                _logger.LogError("Cateory id not found");
                return StatusCode(404,isCategoryIdExist);
            }
            List<ProductResponseDTO> products = _productServices.GetProductList(catalogId, categoryId,size,pageNo,sortBy,sortOrder,name);
            if(products == null)
            {
                return StatusCode(204,"No products found");
            }
            _logger.LogInformation("ALl product retrieved successfully");
            return Ok(products);
        }

        ///<summary>
        /// Gets Product price
        ///</summary>
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/product/{id}/price")]
        public float GetProductPrice([FromRoute] Guid id)
        {
            _logger.LogInformation("GetProductPrice started");
            return _productServices.GetProductPrice(id);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/bill/product/details")]
        public List<ProductBillResponseDTO> GetBillProductDetails([FromBody] List<BillProductDTO> productIds)
        {
            _logger.LogInformation("Get ProductDetails started");
           return _productServices.GetBillProductDetails(productIds);
        }
        ///<summary>
        /// Gets cart product details
        ///</summary>
        ///<return>List<CartResponseDTO></return>
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Route("api/product/details")]
        public List<CartResponseDTO> GetCartProductDetails([FromBody] List<ProductQunatity> products )
        {
            _logger.LogInformation("GetCartProductDetails started");
            return _productServices.GetCartProductDetails(products);
        }

        ///<summary>
        /// Gets Wish List product details
        ///</summary>
        ///<return>List<CartResponseDTO></return>
        [HttpPost]
        [Authorize(Roles ="Admin,User")]
        [Route("/api/wish-list/product/details")]
        public List<CartResponseDTO> GetWishListProductDetails([FromBody] List<Guid> productIds )
        {
            _logger.LogInformation("GetWishListProductDetails started");
            return _productServices.GetWishListProductDetails(productIds);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("/api/category/names")]
        public ActionResult<List<CategoryNamesResponseDTO>> GetCategoryNames()
        {
            List<CategoryNamesResponseDTO> names = _productServices.GetCategoryNames();
            if (names != null)
            {
                return Ok(names);
            }
            return StatusCode(204);
        }
    }
}
