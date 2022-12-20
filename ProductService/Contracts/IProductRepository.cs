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

        public Guid? IsCatelogIdexists(ProductDTO productDTOs);

        public void SaveProduct(List<Product> product) ;

        public Tuple<string, Guid> IsProductExists(Guid id,Guid categoryId);

        public Product GetProduct(Guid id,Guid categoryId);

        public bool UpdateProduct(Product product, Guid CategoryId);

        public bool IsProductNameExists(UpdateProductDTO product);

        public Tuple<string, string> IsExists(CatalogDTO catalogDTO);

        public Guid SaveCatalog(Catalog catalogs);

        public int? GetProductCount(Guid id,Guid categoryId);
    }
}
