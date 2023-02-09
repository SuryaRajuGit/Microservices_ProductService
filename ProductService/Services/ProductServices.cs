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
using System.Reflection;
using System.Text;

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
                type = "BadRequest",
                message = ModelState.Values.Select(src => src.Errors.Select(src => src.ErrorMessage).FirstOrDefault()).FirstOrDefault(),
                statusCode="400"
            };
        }
        public ErrorDTO IsQuantityNotNull(CatalogDTO catalog)
        {
            List<CategoryDTOcatalog> category = catalog.Category.ToList();
            foreach (CategoryDTOcatalog item in category)
            {
                List<CategoryDTO> product = item.Products.ToList();
                foreach (CategoryDTO term in product)
                {
                    if(term.Quantity == 0)
                    {
                        return new ErrorDTO() { type = "BadRequest", message = "Quantity is required",statusCode="400" };
                    }
                }

            }
            return null;
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
                return new ErrorDTO { type = "Conflict", message = names + "Product already exists",statusCode="409" };
            }
            return null;
        }
        ///<summary>
        /// Checks if the catalog id exists
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsCategoryIdExists(ProductDTO product)
        {
            bool isExist = _productRepository.IsCatelogIdexists(product);
            if (!isExist)
            {
                return new ErrorDTO { type = "NotFound", message = "Category id not found",statusCode="404" };
            }
            return null;
        }
        ///<summary>
        /// Saves Product 
        ///</summary>
        ///<return>List<SaveProductResponse></return>
        public void SaveProduct(ProductDTO productDTO)
        {
          
            Guid userId = Guid.Parse(_context.HttpContext.User.Claims.First(i => i.Type == Constants.Id).Value);
            List<Product> products = new List<Product>();
            
            foreach (CategoryDTO item in productDTO.Product)
            {
                item.Quantity = item.Quantity == 0 ? 1 : item.Quantity;
                item.Visibility = item.Visibility == null ? false : true;
                Product product1 = _mapper.Map<Product>(item);
                Guid id = Guid.NewGuid();
                product1.Id = id;
                product1.CategoryId = productDTO.CategoryId;
                product1.CreatedDate = DateTime.Now;
                product1.CreatedBy = userId;
                product1.IsActive = true;
                product1.Asset = item.Asset;
                products.Add(product1);  
            }
            _productRepository.SaveProduct(products);
        }
        ///<summary>
        /// Checks if the product exits or not
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsProductExists(Guid id)
        {
            bool isProductExist = _productRepository.IsProductExists(id);
            if(!isProductExist)
            {
                return new ErrorDTO() {type="Not Found",message="Product id not found",statusCode="404" };
            }
            return null;
        }

        ///<summary>
        /// Gets product details
        ///</summary>
        ///<return>ProductResponseDTO</return>
        public ProductResponseDTO GetProduct(Guid id)
        {
            Product product = _productRepository.GetProduct(id);
            ProductResponseDTO product1 = _mapper.Map<ProductResponseDTO>(product);

            return product1;
        }

        ///<summary>
        /// Updates product details
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO UpdateProduct(UpdateProductDTO product,Guid id)
        {
        
            bool isProductExist = _productRepository.UpdateProduct(product,id);
            if (isProductExist)
            {
                return null;
            }
            return new ErrorDTO { type = "NotFound", message = "Product Id not found",statusCode="404" };
        }

        ///<summary>
        /// Check product name exists or not
        ///</summary>
        ///<return>ErrorDTO</return>
        public ErrorDTO IsProductNameExists(UpdateProductDTO product,Guid id)
        {
            bool isProductExist = _productRepository.IsProductNameExists(product, id);
            if (isProductExist)
            {
                return new ErrorDTO { type = "Conflict", message = "Product with name already exists",statusCode="409" };
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
                    return new ErrorDTO { type = "Conflict", message = $"Catalog with name {response.Item2} already exists",statusCode="409" };
                case Constants.Category:
                    return new ErrorDTO { type = "Conflict", message = $"Category with name {response.Item2} already exists",statusCode="409" };
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
                     IsActive = true,
                     CreatedDate = DateTime.Now,
                     Products = term.Products.Select(item =>
                        new Product()
                        {
                            Id = Guid.NewGuid(),
                            Price = item.Price,
                            Description = item.Description,
                            Quantity = item.Quantity,
                            Visibility = item.Visibility == null ? true : false,
                            Asset = item.Asset,
                            Name = item.Name,
                            CreatedDate =DateTime.Now,
                            IsActive=true
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
        public string GetProductQuantity(Guid id)
        {
            Tuple<string, string> response = _productRepository.GetProductCount(id);
            if(response == null)
            {
                return JsonConvert.SerializeObject(new QuantityResponse { type = "product", description = $"product id {response.Item2} not found" });
            }
            return JsonConvert.SerializeObject(new QuantityResponse() { type = response.Item1, description = response.Item2 });
            
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
                return new ErrorDTO { type = "NoFound", message = "Product with id not found",statusCode="404" };
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
                    return new ErrorDTO { type = "Conflict", message = $"Catalog {isCategoryIdExist.Item2} id not found",statusCode="404" };
                case Constants.Category:
                    return new ErrorDTO { type = "Conflict", message = $"Category {isCategoryIdExist.Item2}id not found",statusCode="404" };
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
                return new ErrorDTO() { type = "NotFound", message = "Product with id not found",statusCode="404" };
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
                return new ErrorDTO() { type = "Bad request", message = "catalog id and category id should not be empty" , statusCode = "404" };
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
                    response.Add(cartResponseDTO);
                }
                else if(item.Name !=null &&  !item.Visibility)
                {
                    CartResponseDTO cartResponseDTO = _mapper.Map<CartResponseDTO>(item);
                    cartResponseDTO.Quantity = 0;
                    response.Add(cartResponseDTO);
                }
                else
                {
                    CartResponseDTO cartResponseDTO = _mapper.Map<CartResponseDTO>(item);
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
                    productDetails.Add(cart);
                }
                else
                {
                    CartResponseDTO cart = _mapper.Map<CartResponseDTO>(item);
                    cart.Quantity = 1;
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
            foreach (Category item in catalog.Category.Where(sel => sel.IsActive))
            {
                GetCatalogResponseDTO getCatalogResponseDTO = new GetCatalogResponseDTO()
                {
                    CategoryId = item.Id,
                    CategoryName=item.Name,
                    Product = new List<ProductResponseDTO>()
                };
                foreach (Product each in item.Products.Where(sel=>sel.IsActive))
                {
                    ProductResponseDTO productResponseDTO1 = _mapper.Map<ProductResponseDTO>(each);
                    getCatalogResponseDTO.Product.Add(productResponseDTO1);
                }
                getCatalogResponseDTOs.Add(getCatalogResponseDTO);
            }
            return getCatalogResponseDTOs;
        }
        public ErrorDTO IsProductQuantityNotNull(int quantity)
        {
            if(quantity == 0)
            {
                return new ErrorDTO() {type="BadRequest",message ="Product Quantity required",statusCode="400" };
            }
            return null;
        }
        public List<CategoryNamesResponseDTO> GetCategoryNames()
        {
            List<CategoryNamesResponseDTO> response = _productRepository.GetCategoryNamesList();
            if(response.Count() == 0)
            {
                return null;
            }
            return response;
        }
    }
    
}
