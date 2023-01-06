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
        public ErrorDTO ModelStateInvalid(ModelStateDictionary ModelState);

        public ErrorDTO IsProductExists(IEnumerable<string> name);

        public ErrorDTO IsCatelogIdexists(ProductDTO product);

        public List<SaveProductResponse> SaveProduct(ProductDTO product);

        public ErrorDTO IsProductExists(Guid id,Guid categoryId);

        public ProductResponseDTO GetProduct(Guid id,Guid categoryId);

        public ErrorDTO UpdateProduct(UpdateProductDTO product);

        public ErrorDTO IsProductNameExists(UpdateProductDTO  updateProductDTO);

        public ErrorDTO IsCatalogNameExists(CatalogDTO catalogDTO);

        public CatalogResponseDTO SaveCatalog(CatalogDTO catalogDTO);

        public string GetProductQuantity(Guid id,Guid categoryId);

        public ErrorDTO IsProductExist(Guid id);

        public Product Product(Guid id);

        public ErrorDTO IsCategoryIdExist(Guid catalogId,Guid categoryId,string name);

        public List<ProductResponseDTO> GetProductList(Guid catalogId,Guid categoryId,int size,int pageNo, string sortBy,string sortOrder,string name);

        public float GetProductPrice(Guid id);

        public ErrorDTO DeleteProduct(Guid id);

        public ErrorDTO ValidateFields(Guid catalogId,Guid categoryId,string name);

        public bool UpdatePurchaseProduct(ProductToCartDTO updatePurchasedProduct);

        public string CheckProducts(List<Guid> updatePurchasedProducts);

        public string CheckProductQunatity(List<ProductQunatity> item);

        public string CartProductsPrice(List<ProductQunatity> products);

        public List<CartResponseDTO> GetCartProductDetails(List<ProductQunatity> products);

        //     public string IsProductQuantityExist();
        public List<CartResponseDTO> GetWishListProductDetails(List<Guid> productIds);
    }
}
