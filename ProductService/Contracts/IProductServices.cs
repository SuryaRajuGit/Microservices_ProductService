using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProductService.Entity.Dto;
using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Contracts
{
    public interface IProductServices
    {
        ///<summary>
        /// Checks received data
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO ModelStateInvalid(ModelStateDictionary ModelState);

        ///<summary>
        /// Checks if the product exists
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsProductExists(IEnumerable<string> name);

        ///<summary>
        /// Checks if the catalog id exists
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsCategoryIdExists(ProductDTO product);

        ///<summary>
        /// Saves Product 
        ///</summary>
        ///<return>List<SaveProductResponse></return>
        public void SaveProduct(ProductDTO product);

        ///<summary>
        /// Checks if the product exits or not
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsProductExists(Guid id);

        ///<summary>
        /// Gets product details
        ///</summary>
        ///<return>ProductResponseDTO</return>
        public ProductResponseDTO GetProduct(Guid id);

        ///<summary>
        /// Updates product details
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO UpdateProduct(UpdateProductDTO product,Guid id);

        ///<summary>
        /// Check product name exists or not
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsProductNameExists(UpdateProductDTO updateProductDTO,Guid id);

        ///<summary>
        /// Checks catalog name exists or not
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsCatalogNameExists(CatalogDTO catalogDTO);

        ///<summary>
        /// Saves catalog details
        ///</summary>
        ///<return>Guid</return>
        public Guid SaveCatalog(CatalogDTO catalogDTO);

        ///<summary>
        /// Gets product quantity 
        ///</summary>
        ///<return>string</return>
        public string GetProductQuantity(Guid id);

        ///<summary>
        /// Checks product exist in the database or not
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsProductExist(Guid id);

        ///<summary>
        /// Gets Product Details
        ///</summary>
        ///<return>Product</return>
        public Product Product(Guid id);

        ///<summary>
        /// Updates purchesed product details in the inventory
        ///</summary>
        ///<return>bool</return>
        public ErrorDTO IsCategoryIdExist(Guid catalogId,Guid categoryId,string name);

        ///<summary>
        /// Gets List of products
        ///</summary>
        ///<return>List<ProductResponseDTO></return>
        public List<ProductResponseDTO> GetProductList(Guid catalogId,Guid categoryId,int size,int pageNo, string sortBy,string sortOrder,string name);

        ///<summary>
        /// Gets the product price
        ///</summary>
        ///<return>float</return>
        public float GetProductPrice(Guid id);

        ///<summary>
        /// Delets Product 
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO DeleteProduct(Guid id);

        ///<summary>
        /// Delets Product 
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO ValidateFields(Guid catalogId,Guid categoryId,string name);

        ///<summary>
        /// Updates purchased product details in the inventory
        ///</summary>
        ///<return>bool</return>
        public bool UpdatePurchaseProduct(ProductToCartDTO updatePurchasedProduct);

        ///<summary>
        /// Checks product details
        ///</summary>
        ///<return>string</return>
        public string CheckProducts(List<Guid> updatePurchasedProducts);

        ///<summary>
        /// Delets Product 
        ///</summary>
        ///<return>ErrorDTO</return>
        public string CheckProductQunatity(List<ProductQunatity> item);

        ///<summary>
        /// Gets Prices of products
        ///</summary>
        ///<return>string</return>
        public string CartProductsPrice(List<ProductQunatity> products);

        ///<summary>
        /// Gets details of products in cart
        ///</summary>
        ///<return>List<CartResponseDTO></return>
        public List<CartResponseDTO> GetCartProductDetails(List<ProductQunatity> products);

        ///<summary>
        /// Gets wishlist product details
        ///</summary>
        ///<return>List<CartResponseDTO></return>
        public List<CartResponseDTO> GetWishListProductDetails(List<Guid> productIds);

        ///<summary>
        /// Gets catalog details
        ///</summary>
        ///<return>List<GetCatalogResponseDTO></return>
        public List<GetCatalogResponseDTO> GetCatalog(Guid id);

         
        public ErrorDTO IsQuantityNotNull(CatalogDTO catalog);

        public ErrorDTO IsProductQuantityNotNull(int quantity);

        public List<CategoryNamesResponseDTO> GetCategoryNames();

    }
}
