using ProductService.Entity.Dto;
using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Contracts
{
    public interface IProductRepository
    {
        public string IsProductExists(List<string> names);

        public bool IsCatelogIdexists(ProductDTO productDTOs);

        public void SaveProduct(List<Product> product) ;

        public bool IsProductExists(Guid id);

        public Product GetProduct(Guid id);

        public bool UpdateProduct(UpdateProductDTO product,Guid id);

        public bool IsProductNameExists(UpdateProductDTO product,Guid id);

        public Tuple<string, string> IsExists(CatalogDTO catalogDTO);

        public void SaveCatalog(Catalog catalogs);

        public Tuple<string, string> GetProductCount(Guid id);

        public bool IsProductExist(Guid id);

        public Product Product(Guid id);

        public Tuple<string,string>IsCategoryExists(Guid catalogId,Guid categoryId);

        public List<Product> GetProductList(string name);

        public List<Product> GetProductsList(Guid catalogId,Guid categoryId,int size,int pageNo);

        public float GetProductPrice(Guid id);

        public bool UpdateProductQuantity(ProductToCartDTO updatePurchasedProduct);

        public int? CheckProductQuantity(ProductToCartDTO item);

        public int GetProductQunatity(Guid item);

        public bool DeleteProduct(Guid id);

        public ProductPrice GetProductsPrice(Guid item,int quantity);

        public bool CheckProduct(Guid item);

        public List<Product> GetCartProductDetails(List<ProductQunatity> products);

        public List<Product> GetWishListProductDetails(List<Guid> productIds);

        public Catalog GetCatalogDetails(Guid id);

        public List<CategoryNamesResponseDTO> GetCategoryNamesList();

        public List<ProductBillResponseDTO> GetProductDetails(List<BillProductDTO> ids);
    }
}
