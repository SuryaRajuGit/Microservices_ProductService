using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using ProductService.Contracts;
using ProductService.Entity.Dto;
using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductService.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IHttpContextAccessor _context;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductServices(IProductRepository productRepository, IMapper mapper, IHttpContextAccessor context)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _context = context;
        }
        ///<summary>
        /// Checks received data
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO ModelStateInvalid(ModelStateDictionary ModelState)
        {
            return new ErrorDTO
            {
                type = ModelState.Keys.FirstOrDefault(),
                description = ModelState.Values.Select(src => src.Errors.Select(src => src.ErrorMessage).FirstOrDefault()).FirstOrDefault()
            };
        }

        ///<summary>
        /// Checks if the product exists
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsProductExists(IEnumerable<string> name)
        {
            List<string> nameList = name.ToList();
            string names = _productRepository.IsProductExists(nameList);
            if (names != null)
            {
                return new ErrorDTO { type = "Product", description = names + "Product already exists" };
            }
            return null;
        }
        ///<summary>
        /// Checks if the catalog id exists
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsCatelogIdexists(ProductDTO product)
        {
            Guid? id = _productRepository.IsCatelogIdexists(product);
            if (id != null)
            {
                return new ErrorDTO { type = "Catalog", description ="Catalog with id not found" };
            }
            return null;
        }
        ///<summary>
        /// Saves Product 
        ///</summary>
        ///<return>List<SaveProductResponse></return>
        public List<SaveProductResponse> SaveProduct(ProductDTO productDTO)
        {
            Guid userId = Guid.Parse(_context.HttpContext.User.Claims.First(i => i.Type == "Id").Value);
            List<Product> products = new List<Product>();
            List<SaveProductResponse> response = new List<SaveProductResponse>();
            foreach (CategoryDTO item in productDTO.Product)
            {
                item.Quantity = item.Quantity == 0 ? 1 : item.Quantity;
                Product product1 = _mapper.Map<Product>(item);
                Guid id = Guid.NewGuid();
                product1.Id = id;
                product1.CategoryId = productDTO.CategoryId;
                product1.CreatedDate = DateTime.Now;
                product1.CreatedBy = userId;
                products.Add(product1);
                SaveProductResponse saveProductResponse = new SaveProductResponse();
                saveProductResponse.Id = id;
                saveProductResponse.Name = item.Name;
                response.Add(saveProductResponse);
            }
            _productRepository.SaveProduct(products);
            return response;
        }
        ///<summary>
        /// Checks if the product exits or not
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsProductExists(Guid id, Guid categoryId)
        {
            Tuple<string, Guid> isProductExists = _productRepository.IsProductExists(id, categoryId);
            if (isProductExists.Item1 != string.Empty)
            {
                if (isProductExists.Item1 == Constants.CategoryId)
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

        ///<summary>
        /// Gets product details
        ///</summary>
        ///<return>ProductResponseDTO</return>
        public ProductResponseDTO GetProduct(Guid id, Guid categoryId)
        {
            Product product = _productRepository.GetProduct(id, categoryId);
            product.Category = null;
            //  product.CategoryId = null;
            ProductResponseDTO productResponseDTO = new ProductResponseDTO();

            ProductResponseDTO product1 = _mapper.Map<ProductResponseDTO>(product);
            product1.Asset = null;

            return product1;
        }

        ///<summary>
        /// Updates product details
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO UpdateProduct(UpdateProductDTO product)
        {
        
            Product product2 = _mapper.Map<Product>(product);
            product2.Asset = null;
            bool isProductExist = _productRepository.UpdateProduct(product, product.CategoryId);
            if (isProductExist)
            {
                return null;
            }
            return new ErrorDTO { type = "Product", description = "Product Id not found" };
        }

        ///<summary>
        /// Check product name exists or not
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsProductNameExists(UpdateProductDTO product)
        {
            bool isProductExist = _productRepository.IsProductNameExists(product);
            if (isProductExist)
            {
                return new ErrorDTO { type = "Product", description = "Product with name already exists" };
            }
            return null;
        }
        ///<summary>
        /// Checks catalog name exists or not
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsCatalogNameExists(CatalogDTO catalogDTO)
        {
            Tuple<string,string> response = _productRepository.IsExists(catalogDTO);
            switch (response.Item1)
            {
                case Constants.Catalog:
                    return new ErrorDTO { type = "Catalog", description = $"Catalog with name {response.Item2} already exists" };
                case Constants.Category:
                    return new ErrorDTO { type = "Category", description = $"Category with name {response.Item2} already exists" };
                default:
                    return null;
            }
        }
        ///<summary>
        /// Saves catalog details
        ///</summary>
        ///<return>Guid</return>
        public Guid SaveCatalog(CatalogDTO catalogDTO)
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
            _productRepository.SaveCatalog(catalogs);   
            return catalogId;
        }
        ///<summary>
        /// Checks product details
        ///</summary>
        ///<return>string</return>
        public string CheckProducts(List<Guid> productIds)
        {
            foreach (Guid item in productIds)
            {
                bool isProductExist = _productRepository.CheckProduct(item);
                if(!isProductExist)
                {
                    return JsonConvert.SerializeObject(item);
                }
            }
            return null;
        }
        ///<summary>
        /// Gets product quantity 
        ///</summary>
        ///<return>string</return>
        public string GetProductQuantity(Guid id, Guid categoryId)
        {
            Tuple<string, string> response = _productRepository.GetProductCount(id, categoryId);
            switch (response.Item1)
            {
                case Constants.Category:
                    return JsonConvert.SerializeObject(new QuantityResponse { type = "category", description = $"Category id {response.Item2}  not found" });
                case Constants.Product:
                    return JsonConvert.SerializeObject(new QuantityResponse { type = "product", description = $"product id {response.Item2} not found" });
                default:
                    return JsonConvert.SerializeObject(new QuantityResponse() { type = response.Item1, description = response.Item2 });
            }
        }
        ///<summary>
        /// Updates purchased product details in the inventory
        ///</summary>
        ///<return>bool</return>
        public bool UpdatePurchaseProduct(ProductToCartDTO updatePurchasedProduct)
        {
            return _productRepository.UpdateProductQuantity(updatePurchasedProduct);
        }
        ///<summary>
        /// Checks product exist in the database or not
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsProductExist(Guid id)
        {
            bool isProductExist = _productRepository.IsProductExist(id);
            if (!isProductExist)
            {
                return new ErrorDTO { type = "NoFound", description = "Product with id not found" };
            }
            return null;
        }

        ///<summary>
        /// Gets Product Details
        ///</summary>
        ///<return>Product</return>
        public Product Product(Guid id)
        {
            return _productRepository.Product(id);
        }

        ///<summary>
        /// Updates purchesed product details in the inventory
        ///</summary>
        ///<return>bool</return>
        public ErrorDTO IsCategoryIdExist(Guid catalogId, Guid categoryId, string name)
        {
            if (name != null)
            {
                return null;
            }
            Tuple<string, string> isCategoryIdExist = _productRepository.IsCategoryExists(catalogId, categoryId);
            switch (isCategoryIdExist.Item1)
            {
                case Constants.Catalog:
                    return new ErrorDTO { type = "Catalog", description = $"Catalog {isCategoryIdExist.Item2} id not found" };
                case Constants.Category:
                    return new ErrorDTO { type = "Category", description = $"Category {isCategoryIdExist.Item2}id not found" };
                default:
                    return null;
            }
        }

        ///<summary>
        /// Updates purchesed product details in the inventory
        ///</summary>
        ///<return>bool</return>
        public List<ProductResponseDTO> ProductResponse(List<Product> products)
        {
            List<ProductResponseDTO> productList = new List<ProductResponseDTO>();
            foreach (Product item in products)
            { 
                ProductResponseDTO product2 = _mapper.Map<ProductResponseDTO>(item);
                product2.Asset = null;
            
            productList.Add(product2);
            }
            return productList;
        }
        ///<summary>
        /// Gets List of products
        ///</summary>
        ///<return>List<ProductResponseDTO></return>
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
                List<Product> productList =  sortList = sortBy == Constants.Name ? products1.OrderBy(term => term.Name).ToList() : products1.OrderBy(term => term.Price).ToList();
                if(sortBy == Constants.Name)
                {
                    List<Product> productsList =  products1.OrderByDescending(term => term.Name).ToList();
                    products1 = productsList;
                }
                else
                {
                    List<Product> productsList = products1.OrderByDescending(term => term.Price).ToList();
                    products1 = productsList;
                }
            }
            sortList = products1;
            List<ProductResponseDTO> response =  ProductResponse(sortList);
            return response; 
        }
        ///<summary>
        /// Gets the product price
        ///</summary>
        ///<return>float</return>
        public float GetProductPrice(Guid id)
        {
            return _productRepository.GetProductPrice(id);
        }

        ///<summary>
        /// Delets Product 
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO DeleteProduct(Guid id)
        {
            bool isProductExist = _productRepository.DeleteProduct(id);
            if (isProductExist == false)
            {
                return new ErrorDTO() { type = "Product", description = "Product with id not found" };
            }
            return null;
        }

        ///<summary>
        /// Delets Product 
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO ValidateFields(Guid catalogId, Guid categoryId, string name)
        {
            if (name == null && (catalogId == Guid.Empty || categoryId == Guid.Empty)) 
            {
                return new ErrorDTO() { type = "Bad request", description = "catalog id and category id should not be empty" };
            }
            return null;
        }

        ///<summary>
        /// Delets Product 
        ///</summary>
        ///<return>ErrorDTO</return>
        public string CheckProductQunatity(List<ProductQunatity> productQunatities)
        {
            foreach (ProductQunatity item in productQunatities)
            {
                int productCount = _productRepository.GetProductQunatity(item.Id);
                if(productCount == -1)
                {
                    return JsonConvert.SerializeObject(new ProductQunatity() { Id = item.Id, Quantity = -1 });
                }
                else if (productCount < item.Quantity)
                {
                    return JsonConvert.SerializeObject(new ProductQunatity() {Id=item.Id,Quantity= productCount });
                }
            }
            return null;
        }
        ///<summary>
        /// Gets Prices of products
        ///</summary>
        ///<return>string</return>
        public string CartProductsPrice(List<ProductQunatity> products)
        {
            List<ProductPrice> productPrices = new List<ProductPrice>();
            foreach (ProductQunatity item in products)
            {
                ProductPrice productPrice = _productRepository.GetProductsPrice(item.Id,item.Quantity);
                productPrices.Add(productPrice);
            }
            return JsonConvert.SerializeObject(productPrices);
        }

        ///<summary>
        /// Gets details of products in cart
        ///</summary>
        ///<return>List<CartResponseDTO></return>
        public List<CartResponseDTO> GetCartProductDetails(List<ProductQunatity> products)
        {
            List<Product> productList = _productRepository.GetCartProductDetails(products);
            List<CartResponseDTO> response = new List<CartResponseDTO>();
            foreach (Product item in productList)
            {
                if(item.Name != null || item.Visibility )
                {
                    CartResponseDTO cartResponseDTO = _mapper.Map<CartResponseDTO>(item);
                    cartResponseDTO.Asset = null;
                    response.Add(cartResponseDTO);
                }
                else if(item.Name !=null &&  !item.Visibility)
                {
                    CartResponseDTO cartResponseDTO = _mapper.Map<CartResponseDTO>(item);
                    cartResponseDTO.Asset = null;
                    cartResponseDTO.Quantity = 0;
                    response.Add(cartResponseDTO);
                }
                else
                {
                    CartResponseDTO cartResponseDTO = _mapper.Map<CartResponseDTO>(item);
                    cartResponseDTO.Asset = null;
                    cartResponseDTO.Quantity = -1;
                    response.Add(cartResponseDTO);
                }
            }
            return response;
        }
        ///<summary>
        /// Gets wishlist product details
        ///</summary>
        ///<return>List<CartResponseDTO></return>
        public List<CartResponseDTO> GetWishListProductDetails(List<Guid> productIds)
        {
            List<Product> productList = _productRepository.GetWishListProductDetails(productIds);
            List<CartResponseDTO> productDetails = new List<CartResponseDTO>();
            foreach (Product item in productList)
            {
                
                if (item.Name == null || !item.Visibility)
                {
                    CartResponseDTO cart = new CartResponseDTO();
                    cart.Id = item.Id;
                    cart.Name = null;
                    productDetails.Add(cart);
                }
                else if((item.Quantity == 0 || item.Quantity < 0) && item.Visibility)
                {
                    CartResponseDTO cart = _mapper.Map<CartResponseDTO>(item);
                    cart.Quantity = -1;
                    cart.Asset = null;
                    productDetails.Add(cart);
                }
                else
                {
                    CartResponseDTO cart = _mapper.Map<CartResponseDTO>(item);
                    cart.Quantity = 1;
                    cart.Asset = null;
                    productDetails.Add(cart);
                }
                
            }
            return productDetails;
        }
        ///<summary>
        /// Gets catalog details
        ///</summary>
        ///<return>List<GetCatalogResponseDTO></return>
        public List<GetCatalogResponseDTO> GetCatalog(Guid id)
        {
            Catalog catalog = _productRepository.GetCatalogDetails(id); 
            if(catalog == null)
            {
                return null;
            }
            List<GetCatalogResponseDTO> getCatalogResponseDTOs = new List<GetCatalogResponseDTO>();
            foreach (Category item in catalog.Category)
            {
                GetCatalogResponseDTO getCatalogResponseDTO = new GetCatalogResponseDTO()
                {
                    CategoryId = item.Id,
                    CategoryName=item.Name,
                    Product = new List<ProductResponseDTO>()
                };
                foreach (Product each in item.Products)
                {
                    ProductResponseDTO productResponseDTO1 = _mapper.Map<ProductResponseDTO>(each);
                    productResponseDTO1.Asset = null;
                    getCatalogResponseDTO.Product.Add(productResponseDTO1);
                }
                getCatalogResponseDTOs.Add(getCatalogResponseDTO);
            }
            return getCatalogResponseDTOs;
        }
    }
    
}
