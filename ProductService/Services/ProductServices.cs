using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProductService.Contracts;
using ProductService.Entity.Dto;
using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IProductRepository _productRepository;

        public ProductServices(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public ErrorDTO ModelStateInvalid(ModelStateDictionary ModelState)
        {
            return new ErrorDTO
            {
                type = ModelState.Keys.FirstOrDefault(),
                description = ModelState.Values.Select(src => src.Errors.Select(src => src.ErrorMessage).FirstOrDefault()).FirstOrDefault()
            };
        }
        public ErrorDTO IsProductExists(IEnumerable<string> name)
        {
            var l = name.ToList();
            string names = _productRepository.IsProductExists(l);
            if(names != null)
            {
                return new ErrorDTO { type = "Conflict", description = names + " Product already exists" };
            }
            return null;
        }
        public ErrorDTO IsCatelogIdexists(ProductDTO product)
        {
            Guid? id = _productRepository.IsCatelogIdexists(product); 
            if(id != null)
            {
                return new ErrorDTO {type="NotFound",description=id+" not found" };
            }
            return null;
        }

        public List<Guid> SaveProduct(ProductDTO productDTO)
        {
            List<Guid> ProductIds = new List<Guid>();
            List<Product> products = new List<Product>();
            foreach (var item in productDTO.Categories)
            {
                Product product = new Product();
                Guid id = Guid.NewGuid();
                product.Id = id;
                product.Asset = null;
                product.CategoryId = productDTO.Category_id;
                product.Description = item.Description;
                product.Name = item.Name;
                product.Price = item.Price;
                product.Quantity = item.Quantity;
                products.Add(product);
                ProductIds.Add(id);
            }
            _productRepository.SaveProduct(products);
            return ProductIds;
        }
        public ErrorDTO IsProductExists(Guid id, Guid categoryId)
        {
            Tuple<string, Guid> isProductExists = _productRepository.IsProductExists(id,categoryId); 
            if(isProductExists != null)
            {
                if(isProductExists.Item1 == "categoryId")
                {
                    return new ErrorDTO {type="NotFound",description= "categoryId "+ isProductExists.Item2+" not found" };
                }
                else
                {
                    return new ErrorDTO { type = "NotFound", description = "product with id " + isProductExists.Item2 + " not found" };
                }
            }
            return null;
        }

        public Product GetProduct(Guid id, Guid categoryId)
        {
            Product productResponseDTO = _productRepository.GetProduct(id,categoryId);
            return productResponseDTO;
        }

        public ErrorDTO UpdateProduct(UpdateProductDTO product)
        {
            Product product1 = new Product
            {
                Price = product.Price,
                Name = product.Name,
                Description = product.Description,
                Quantity = product.Quantity,
                Visibility = product.Visibility,
                CategoryId = product.CategoryId,
                Id=product.ProductId
            };
            var c = _productRepository.UpdateProduct(product1,product.CategoryId);
            if(c)
            {
                return null;
            }
            return new ErrorDTO {type="NotFound",description="Product Id not found" };
        }

        public ErrorDTO IsProductNameExists(UpdateProductDTO product)
        {
            var v = _productRepository.IsProductNameExists(product);
            if(v)
            {
                return new ErrorDTO {type="Conflict",description="Product with name already exists" };
            }
            return null;
        }

        public ErrorDTO IsCatalogNameExists(CatalogDTO catalogDTO)
        {
            //
            var c = _productRepository.IsExists(catalogDTO);
            switch(c.Item1)
            {
                case "catalog":
                    return new ErrorDTO {type="Conflict",description=$"Catalog with name {c.Item2} already exists" };
                case "category": 
                    return new ErrorDTO { type = "Conflict", description = $"Category with name {c.Item2} already exists" };
                default:
                    return null;
            }
        }
        public Guid SaveCatalog(CatalogDTO catalogDTO)
        {
            Guid catalogId = Guid.NewGuid();
            Catalog catalogs = new Catalog();
            Category category = new Category();
            catalogs.Id = catalogId;
            foreach (var items in catalogDTO.Category)
            {
                Guid id = Guid.NewGuid();
                
                category.Id = id;
                category.CatalogId = catalogId;
                category.Name = items.Name;
                category.Products.Select(s =>
                new Product
                {
                    Name = s.Name,
                    Price = s.Price,
                    Description = s.Description,
                  //  Asset = s.Asset,
                    Quantity = s.Quantity,
                    Visibility = s.Visibility,
                    CategoryId = id,
                    Id = Guid.NewGuid(),
            }).ToList();
              //  category.Products.Add(product);


                //List<Product> products = new List<Product>();
                //foreach (var item in items.Products)
                //{
                //    Product product = new Product();
                    
                //    product.Name = item.Name;
                //    product.Price = item.Price;
                //    product.Description = item.Description;
                //    product.Asset = item.Asset;
                //    product.Quantity = item.Quantity;
                //    product.Visibility = item.Visibility;
                //    product.CategoryId = id;
                //    product.Id = Guid.NewGuid();
                    
                    
                //    //products.Add(product);
                //    category.Products.Add(product);
                //}
                
              //  catalogs.Category.Add(category);
            }
            //category.Products.Add(products);
            catalogs.Category.Add(category);
            
            return _productRepository.SaveCatalog(catalogs);
        }

        public int? GetProductQuantity(Guid id,Guid categoryId)
        {
            return _productRepository.GetProductCount(id, categoryId);
       
        }
    }
}
