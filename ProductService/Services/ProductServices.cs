using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
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
            if (names != null)
            {
                return new ErrorDTO { type = "Conflict", description = names + " Product already exists" };
            }
            return null;
        }
        public ErrorDTO IsCatelogIdexists(ProductDTO product)
        {
            Guid? id = _productRepository.IsCatelogIdexists(product);
            if (id != null)
            {
                return new ErrorDTO { type = "NotFound", description = id + " not found" };
            }
            return null;
        }

        public List<SaveProductResponse> SaveProduct(ProductDTO productDTO)
        {
            List<Product> products = new List<Product>();
            List<SaveProductResponse> response = new List<SaveProductResponse>();
            foreach (var item in productDTO.Product)
            {
                Product product = new Product();
                Guid id = Guid.NewGuid();
                product.Id = id;
                product.Asset = null;
                product.CategoryId = productDTO.CategoryId;
                product.Description = item.Description;
                product.Name = item.Name;
                product.Price = item.Price;
                product.Quantity = item.Quantity;
                product.Visibility = item.Visibility;
                products.Add(product);
                SaveProductResponse saveProductResponse = new SaveProductResponse();
                saveProductResponse.Id = id;
                saveProductResponse.Name = item.Name;
                response.Add(saveProductResponse);
            }
            _productRepository.SaveProduct(products);
            return response;
        }
        public ErrorDTO IsProductExists(Guid id, Guid categoryId)
        {
            Tuple<string, Guid> isProductExists = _productRepository.IsProductExists(id, categoryId);
            if (isProductExists.Item1 != string.Empty)
            {
                if (isProductExists.Item1 == "categoryId")
                {
                    return new ErrorDTO { type = "Category", description = "categoryId " + isProductExists.Item2 + " not found" };
                }
                else
                {
                    return new ErrorDTO { type = "Product", description = "product with id " + isProductExists.Item2 + " not found" };
                }
            }
            return null;
        }

        public ProductResponseDTO GetProduct(Guid id, Guid categoryId)
        {
            Product product = _productRepository.GetProduct(id, categoryId);
            ProductResponseDTO productResponseDTO1 = new ProductResponseDTO()
            {
                Id=product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Asset=product.Asset
            };
            return productResponseDTO1;
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
                Id = product.ProductId
            };
            var c = _productRepository.UpdateProduct(product1, product.CategoryId);
            if (c)
            {
                return null;
            }
            return new ErrorDTO { type = "Product", description = "Product Id not found" };
        }

        public ErrorDTO IsProductNameExists(UpdateProductDTO product)
        {
            var v = _productRepository.IsProductNameExists(product);
            if (v)
            {
                return new ErrorDTO { type = "Product", description = "Product with name already exists" };
            }
            return null;
        }

        public ErrorDTO IsCatalogNameExists(CatalogDTO catalogDTO)
        {
            //
            var c = _productRepository.IsExists(catalogDTO);
            switch (c.Item1)
            {
                case "catalog":
                    return new ErrorDTO { type = "Catalog", description = $"Catalog with name {c.Item2} already exists" };
                case "category":
                    return new ErrorDTO { type = "Category", description = $"Category with name {c.Item2} already exists" };
                default:
                    return null;
            }
        }
        public CatalogResponseDTO SaveCatalog(CatalogDTO catalogDTO)
        {
            Guid catalogId = Guid.NewGuid();
            Catalog catalogs = new Catalog()
            {
                Id = catalogId,
                Category = catalogDTO.Category.Select(term =>
                 new Category()
                 {
                     Id = Guid.NewGuid(),
                     CatalogId = catalogId,
                     Name = term.Name,
                     Products = term.Products.Select(item =>
                        new Product()
                        {
                            Id = Guid.NewGuid(),
                            Price = item.Price,
                            Description = item.Description,
                            Quantity = item.Quantity,
                            Visibility = item.Visibility,
                            Asset = item.Asset,
                            Name = item.Name,

                        }

                        ).ToList()
                 }).ToList(),
                Name = catalogDTO.CatalogName
            };
            CatalogResponseDTO catalogResponseDTO = new CatalogResponseDTO();
            catalogResponseDTO.CatalogName = catalogDTO.CatalogName;
            catalogResponseDTO.CatalogId = catalogId;
            catalogResponseDTO.Category = new List<CatelogCategoryDTO>();
            foreach (var item in catalogs.Category)
            {
                List<CatalogProductResponseDTO> catalogProductResponseDTO = new List<CatalogProductResponseDTO>();
                foreach (var each in item.Products)
                {
                    CatalogProductResponseDTO catalogProductResponseDTO1 = new CatalogProductResponseDTO()
                    {
                        Id = each.Id,
                        Name = each.Name,
                    };
                    catalogProductResponseDTO.Add(catalogProductResponseDTO1);
                };
                CatelogCategoryDTO catelogCategoryDTO = new CatelogCategoryDTO()
                {
                    CategoryName = item.Name,
                    CategoryId = item.Id,
                    Product = catalogProductResponseDTO,
                };
                catalogResponseDTO.Category.Add(catelogCategoryDTO);
            };
            _productRepository.SaveCatalog(catalogs);
            return catalogResponseDTO;
        }
        public string CheckProducts(List<Guid> productIds)
        {
            foreach (var item in productIds)
            {
                var i = _productRepository.CheckProduct(item);
                if(!i)
                {
                    return JsonConvert.SerializeObject(item);
                }
            }
            return null;
        }
        public string GetProductQuantity(Guid id, Guid categoryId)
        {
            Tuple<string, string> response = _productRepository.GetProductCount(id, categoryId);
            switch (response.Item1)
            {
                case "category":
                    return JsonConvert.SerializeObject(new QuantityResponse { type = "category", description = $"Category id {response.Item2}  not found" });
                case "product":
                    return JsonConvert.SerializeObject(new QuantityResponse { type = "product", description = $"product id {response.Item2} not found" });
                default:
                    return JsonConvert.SerializeObject(new QuantityResponse() { type = response.Item1, description = response.Item2 });
            }
        }
        public bool UpdatePurchaseProduct(ProductToCartDTO updatePurchasedProduct)
        {
            return _productRepository.UpdateProductQuantity(updatePurchasedProduct);
        }



        public ErrorDTO IsProductExist(Guid id)
        {
            var product = _productRepository.IsProductExist(id);
            if (!product)
            {
                return new ErrorDTO { type = "NoFound", description = "Product with id not found" };
            }
            return null;
        }
        public Product Product(Guid id)
        {
            return _productRepository.Product(id);
        }

        public ErrorDTO IsCategoryIdExist(Guid catalogId, Guid categoryId, string name)
        {
            if (name != "")
            {
                return null;
            }
            Tuple<string, string> isCategoryIdExist = _productRepository.IsCategoryExists(catalogId, categoryId);
            switch (isCategoryIdExist.Item1)
            {
                case "catalog":
                    return new ErrorDTO { type = "Catalog", description = $"Catalog with name {isCategoryIdExist.Item2} already exists" };
                case "category":
                    return new ErrorDTO { type = "Category", description = $"Category with name {isCategoryIdExist.Item2} already exists" };
                default:
                    return null;
            }
        }
        public List<ProductResponseDTO> ProductResponse(List<Product> products)
        {
            List<ProductResponseDTO> productList = new List<ProductResponseDTO>();
            foreach (var item in products)
            {
                ProductResponseDTO productDTO = new ProductResponseDTO()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Asset = item.Asset,
                    Price = item.Price,
                    Quantity = item.Quantity
                };
                productList.Add(productDTO);
            }
            return productList;
        }
        public List<ProductResponseDTO> GetProductList(Guid catalogId, Guid categoryId, int size, int pageNo , string sortBy , string sortOrder, string name)
        {
            if (name != null)
            {
                List<Product> products = _productRepository.GetProductList(name);
                if (products.Count() == 0)
                {
                    return null;
                }
                return ProductResponse(products);
            }
            List<Product> products1 = _productRepository.GetProductsList(catalogId, categoryId, size, pageNo);
            List<Product> sortList;
            if (products1 == null)
            {
                return null;
            }
            if(sortOrder == Constants.DSC)
            {
                var x =  sortList = sortBy == Constants.Name ? products1.OrderBy(term => term.Name).ToList() : products1.OrderBy(term => term.Price).ToList();
                if(sortBy == Constants.Name)
                {
                    var xx =  products1.OrderByDescending(term => term.Name).ToList();
                    products1 = x;
                }
                else
                {
                    var y = products1.OrderByDescending(term => term.Price).ToList();
                    products1 = y;
                }
            }
            
            sortList = products1;
            var r =  ProductResponse(sortList);
            return r; 
        }
        public float GetProductPrice(Guid id)
        {
            return _productRepository.GetProductPrice(id);
        }

        public ErrorDTO DeleteProduct(Guid id)
        {
            var c = _productRepository.DeleteProduct(id);
            if (c == false)
            {
                return new ErrorDTO() { type = "Product", description = "Product with id not found" };
            }
            return null;
        }

        public ErrorDTO ValidateFields(Guid catalogId, Guid categoryId, string name)
        {
            if (name == null && catalogId == Guid.Empty && categoryId == Guid.Empty)
            {
                return new ErrorDTO() { type = "Bad request", description = "catalog id and category id should not be empty" };
            }
            return null;

        }
        public string CheckProductQunatity(List<ProductQunatity> productQunatities)
        {
            foreach (var item in productQunatities)
            {
                var t = _productRepository.GetProductQunatity(item.Id);
                if(t == -1)
                {
                    return JsonConvert.SerializeObject(new ProductQunatity() { Id = item.Id, Quantity = -1 });
                }
                else if (t < item.Quantity)
                {
                    return JsonConvert.SerializeObject(new ProductQunatity() {Id=item.Id,Quantity=t });
                }
            }
            return null;
        }
        public string CartProductsPrice(List<ProductQunatity> products)
        {
            List<ProductPrice> productPrices = new List<ProductPrice>();
            foreach (var item in products)
            {
                int i = item.Quantity;
                var y = _productRepository.GetProductsPrice(item.Id,item.Quantity);
                productPrices.Add(y);
            }
            return JsonConvert.SerializeObject(productPrices);
        }
        public List<CartResponseDTO> GetCartProductDetails(List<ProductQunatity> products)
        {
            List<Product> g = _productRepository.GetCartProductDetails(products);
            List<CartResponseDTO> response = new List<CartResponseDTO>();
            foreach (var item in g)
            {
                if(item.Name != null || item.Visibility )
                {
                    CartResponseDTO product = new CartResponseDTO
                    {
                        Id = item.Id,
                        Asset = item.Asset,
                        Name = item.Name,
                        Description=item.Description,
                        Price=item.Price,
                        Quantity=item.Quantity 
                    };
                    response.Add(product);
                }
                else if(item.Name !=null &&  !item.Visibility)
                {
                    CartResponseDTO product = new CartResponseDTO
                    {
                        Id = item.Id,
                        Asset = item.Asset,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        Quantity = 0
                    };
                    response.Add(product);
                }
                else
                {
                    CartResponseDTO product = new CartResponseDTO
                    {
                        Id = item.Id,
                        Asset = item.Asset,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        Quantity = -1

                    };
                    response.Add(product);
                }
            }
            return response;
        }
        public List<CartResponseDTO> GetWishListProductDetails(List<Guid> productIds)
        {
            var x = _productRepository.GetWishListProductDetails(productIds);
            List<CartResponseDTO> productDetails = new List<CartResponseDTO>();
            foreach (var item in x)
            {
                CartResponseDTO cartResponseDTO = new CartResponseDTO();
                if (item.Name == null || !item.Visibility)
                {
                    
                    cartResponseDTO.Id = item.Id;
                    cartResponseDTO.Name = null;
                }
                else if((item.Quantity == 0 || item.Quantity < 0) && item.Visibility)
                {
                   
                    cartResponseDTO.Id = item.Id;
                    cartResponseDTO.Name = item.Name;
                    cartResponseDTO.Price = item.Price;
                    cartResponseDTO.Description = item.Description;
                    cartResponseDTO.Asset = item.Asset;
                    cartResponseDTO.Quantity = -1;
                }
                else
                {
                    cartResponseDTO.Id = item.Id;
                    cartResponseDTO.Name = item.Name;
                    cartResponseDTO.Price = item.Price;
                    cartResponseDTO.Description = item.Description;
                    cartResponseDTO.Asset = item.Asset;
                    cartResponseDTO.Quantity = 1;

                }
                productDetails.Add(cartResponseDTO);
            }
            return productDetails;
        }
    }
}
