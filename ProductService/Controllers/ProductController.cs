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
        [Route("api/admin/product/ocelot")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<Guid>> AddProduct([FromBody] ProductDTO product)
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
            ErrorDTO isproductexists = _productServices.IsProductExists(product.Categories.Select(item => item.Name));
            if (isproductexists != null)
            {
                return StatusCode(409, isproductexists);
            }
            List<Guid> ids = _productServices.SaveProduct(product);
            return StatusCode(201, ids);
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        [Route("api/product/{id}/category/{category-id}")]
        public IActionResult GetProduct([FromRoute(Name = "id")] Guid id, [FromQuery(Name = "category-id")] Guid categoryId )
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
            Product response = _productServices.GetProduct(id, categoryId);
            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("api/admin/catalog/ocelot")]
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
        [Route("api/admin/ocelot")]
        public IActionResult CreateCatalog([FromBody] CatalogDTO catalogDTO)
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
            Guid saveCatalog = _productServices.SaveCatalog(catalogDTO);
            return Ok(saveCatalog);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/product/ocelot")]
        public bool IsProductExistsInventory([FromQuery]Guid id,[FromQuery] Guid categoryId )
        {
            return _productServices.GetProductQuantity(id, categoryId);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [Route("api/product/{id}/category/{category-id}")]
        public IActionResult GetSingleProduct([FromBody] Guid id)
        {
            if (!ModelState.IsValid)
            {
                ErrorDTO badRequest = _productServices.ModelStateInvalid(ModelState);
                return BadRequest(badRequest);
            }
            ErrorDTO isProductExists = _productServices.IsProductExist(id);
            if(isProductExists != null)
            {
                return StatusCode(404, isProductExists);
            }
            Product product = _productServices.Product(id);
            return Ok(product);
        }

        


    }
}
