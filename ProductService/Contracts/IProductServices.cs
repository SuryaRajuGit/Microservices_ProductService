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

        public List<Guid> SaveProduct(ProductDTO product);

        public ErrorDTO IsProductExists(Guid id,Guid categoryId);

        public Product GetProduct(Guid id,Guid categoryId);

        public ErrorDTO UpdateProduct(UpdateProductDTO product);

        public ErrorDTO IsProductNameExists(UpdateProductDTO  updateProductDTO);

        public ErrorDTO IsCatalogNameExists(CatalogDTO catalogDTO);

        public Guid SaveCatalog(CatalogDTO catalogDTO);

        public int? GetProductQuantity(Guid id,Guid categoryId);
    }
}
