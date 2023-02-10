using ProductService.Contracts;
using ProductService.Entity.Dto;
using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ProductService.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _productContext;
        public ProductRepository(ProductContext productContext)
        {
            _productContext = productContext;
        }

        ///<summary>
        /// Checks product name exist or not
        ///</summary>
        ///<return>string</return>
        public string IsProductExists(List<string> names)
        {
            List<string> productNames = _productContext.Product.Where(item => item.IsActive == true).Select(sel => sel.Name).ToList();
            foreach (string item in names)
            {
                if (productNames.Contains(item))
                {
                    return item;
                }
            }
            return null;
        }

        ///<summary>
        /// Checks catelog id exist or not
        ///</summary>
        ///<return>Guid?</return>
        public bool IsCatelogIdexists(ProductDTO productDTOs)
        {
            return _productContext.Category.Any(sel => sel.Id == productDTOs.CategoryId && sel.IsActive);
        }

        ///<summary>
        /// Saves List of products
        ///</summary>
        public void SaveProduct(List<Product> product)
        {
            _productContext.Product.AddRange(product);
            _productContext.SaveChanges();
        }

        ///<summary>
        /// Gets catalog details
        ///</summary>
        ///<return>List<GetCatalogResponseDTO></return>
        public bool IsProductExists(Guid id)
        {
            return _productContext.Product.Any(sel => sel.IsActive && sel.Id == id);
        }

        ///<summary>
        /// Gets Product details
        ///</summary>
        ///<return>Product</return>
        public Product GetProduct(Guid id)
        {
            return _productContext.Product.Where(sel => sel.Id == id).FirstOrDefault();
        }

        ///<summary>
        /// Gets Product quantity
        ///</summary>
        ///<return>int</return>
        public int GetProductQunatity(Guid item)
        {
            Product product = _productContext.Product.Where(find => find.Id == item && find.Visibility && find.IsActive).FirstOrDefault(); 
            if(product == null)
            {
                return -1;
            }
            return product.Quantity;
        }

        ///<summary>
        /// updates product details
        ///</summary>
        ///<return>bool</return>
        public bool UpdateProduct(UpdateProductDTO productDto, Guid id)
        {
            Product productInDb = _productContext.Product.FirstOrDefault(find => find.Id ==id && find.IsActive);
            if (productInDb == null)
            {
                return false;
            }
            PropertyInfo[] properties =  typeof(UpdateProductDTO).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
                object propertyValue = property.GetValue(productDto);

                if(propertyValue != null)
                {
                    productInDb.GetType().GetProperty(propertyName).SetValue(productInDb, propertyValue);
                }
            }
            productDto.Quantity = productDto.Quantity == 0 ? productInDb.Quantity : productDto.Quantity;
            productInDb.UpdateDate = DateTime.Now;
            _productContext.SaveChanges();
            return true;
        }

        ///<summary>
        /// Gets catalog details
        ///</summary>
        ///<return>bool</return>
        public bool IsProductNameExists(UpdateProductDTO product,Guid id)
        {
            return _productContext.Product.Where(find => find.Id != id && find.IsActive).Any(find => find.Name == product.Name);
        }

        ///<summary>
        /// Checks if the catalog exits or not
        ///</summary>
        ///<return>Tuple<string,string></return>
        public Tuple<string,string> IsExists(CatalogDTO catalogDTO)
        {
            bool isCatalogExist = _productContext.Catalog.Any(find => find.Name == catalogDTO.CatalogName && find.IsActive);
            if(isCatalogExist)
            {
                return new Tuple<string, string>(Constants.Catalog,catalogDTO.CatalogName);
            }

            List<string> categoryList = _productContext.Category.Where(sel =>sel.IsActive).Select(item => item.Name).ToList();

            foreach (CategoryDTOcatalog item in catalogDTO.Category)
            {
                if(categoryList.Contains(item.Name.ToString()))
                {
                    return new Tuple<string, string>(Constants.Category, item.Name.ToString());
                }
            }
            return new Tuple<string, string>(string.Empty, string.Empty); 
        }

        ///<summary>
        /// Saves catalog details
        ///</summary>
        public void SaveCatalog(Catalog catalogs)
        {
            _productContext.Catalog.Add(catalogs);
            _productContext.SaveChanges();
        }
        public List<ProductBillResponseDTO> GetProductDetails(List<BillProductDTO> ids)
        {
            List<ProductBillResponseDTO> list1 = new List<ProductBillResponseDTO>();
            foreach (BillProductDTO item in ids)
            {
                ProductBillResponseDTO list = new ProductBillResponseDTO();
                list.Products = new List<ProductDetailsDto>();
                list.BillId = item.BillId;
                foreach (Guid term in item.ProductIds)
                {
                    Product product = _productContext.Product.Where(sel => sel.Id == term && sel.IsActive).FirstOrDefault();
                    if (product != null)
                    {
                        ProductDetailsDto product1 = new ProductDetailsDto()
                        {
                            Id=product.Id,
                            Asset=product.Asset,
                            Description=product.Description,
                            Name=product.Name
                        };
                        list.Products.Add(product1);
                    }
                }
                list1.Add(list);
            }
            return list1;
        }
        ///<summary>
        /// Updates product quantity of product in inventory
        ///</summary>
        ///<return>bool</return>
        public bool UpdateProductQuantity(ProductToCartDTO updatePurchasedProduct)
        {
            Product product = _productContext.Product.Where(find => find.Id == updatePurchasedProduct.ProductId).FirstOrDefault();
            if(product == null)
            {
                return false;
            }
            product.Quantity = product.Quantity - updatePurchasedProduct.Quantity;
            _productContext.Product.Update(product);
            _productContext.SaveChanges();
            return true;
        }

        ///<summary>
        /// Checks product quantity
        ///</summary>
        ///<return>int?</return>
        public int? CheckProductQuantity(ProductToCartDTO item)
        {
            Category category = _productContext.Category.Include(term => term.Products.Where(sel => sel.IsActive)).Where(find => find.Id == item.CategoryId).First();
            Product product = category.Products.Where(find => find.Id == item.ProductId).First();
            if(product.Quantity < item.Quantity)
            {
                return product.Quantity;
            }
            return null;
        }

        ///<summary>
        /// checks if the product exists or not
        ///</summary>
        ///<return>bool</return>
        public bool CheckProduct(Guid id)
        {
            return _productContext.Product.Any(find => find.Visibility && find.Id == id && find.IsActive);
        }

        ///<summary>
        /// Gets product count 
        ///</summary>
        ///<return>Tuple<string,string></return>
        public Tuple<string,string> GetProductCount(Guid id)
        { 
            Product product = _productContext.Product.Where(find => find.Id == id && find.Visibility && find.IsActive).FirstOrDefault();
            if (product == null) 
            {
                return null;
            }
            return new Tuple<string, string>(product.Price.ToString(), product.Quantity.ToString());
        }

        ///<summary>
        /// checks of the product exist or not 
        ///</summary>
        ///<return>bool</return>
        public bool IsProductExist(Guid id)
        {
            return _productContext.Product.Any(find => find.Id == id && find.IsActive);
        }

        ///<summary>
        /// returns product details
        ///</summary>
        ///<return>Product</return>
        public Product Product(Guid id)
        {
            return _productContext.Product.First(find => find.Id == id && find.IsActive);
        }

        ///<summary>
        /// Checks if the caregory exists or not
        ///</summary>
        ///<return>Tuple<string,string></return>
        public Tuple<string, string> IsCategoryExists(Guid catalogId, Guid categoryId)
        {
            Catalog catalog = _productContext.Catalog.Include(term => term.Category).Where(find => find.Id == catalogId && find.IsActive).FirstOrDefault();
            if(catalog == null)
            {
                return new Tuple<string, string>(Constants.Catalog,catalogId.ToString());
            }
            bool isCategoryExist = catalog.Category.Any(find => find.Id == categoryId && find.IsActive);
            if(!isCategoryExist)
            {
                return new Tuple<string, string>(Constants.Category, categoryId.ToString());
            }
            return new Tuple<string, string>(string.Empty, string.Empty); 
        }

        ///<summary>
        /// Gets list of products
        ///</summary>
        ///<return>List<Product></return>
        public List<Product> GetProductList(string name)
        {
            return _productContext.Product.Where(item => item.Name.ToLower().Contains(name.ToLower())).Where(term => term.Visibility == true && term.IsActive).ToList();
        }

        ///<summary>
        /// Gets Producst List 
        ///</summary>
        ///<return>List<Product></return>
        public List<Product> GetProductsList(Guid catalogId, Guid categoryId, int size, int pageNo)
        {
            Catalog catalog = _productContext.Catalog.Include(s => s.Category).ThenInclude(s => s.Products).Where(find => find.Id == catalogId).First();
            Category category = catalog.Category.Where(find => find.Id == categoryId).First();
            ICollection<Product> productList = category.Products;
            productList.Select(find => find.Category = null);
            List<Product> productsList = productList.Skip((pageNo - 1) * 5)
              .Take(size).ToList();
            return productsList;
        }

        ///<summary>
        /// Gets product prices
        ///</summary>
        ///<return>float</return>
        public float GetProductPrice(Guid id)
        {
            return _productContext.Product.Where(find => find.Id == id && find.IsActive && find.Visibility).Select(sel => sel.Price).First();
        }
        ///<summary>
        /// Deletes product in the inventory
        ///</summary>
        ///<return>bool</return>
        public bool DeleteProduct(Guid id)
        {
            Product product = _productContext.Product.Where(find => find.Id == id && find.Visibility && find.IsActive).FirstOrDefault();
            if(product == null)
            {
                return false;
            }
            product.IsActive = false;
            _productContext.Product.Update(product);
            _productContext.SaveChanges();
            return true;
        }

        ///<summary>
        /// Gets product prices
        ///</summary>
        ///<return>ProductPrice</return>
        public ProductPrice GetProductsPrice(Guid id,int quantity)
        {
            Product product = _productContext.Product.Where(find => find.Id == id && find.IsActive).First();
            
            ProductPrice productPrice = new ProductPrice()
            {
                Id= product.Id,
                Price= product.Price
            };
            product.Quantity = product.Quantity - quantity;
            product.UpdateDate = DateTime.Now;
            _productContext.Product.Update(product);
            _productContext.SaveChanges();
            return productPrice;
        }

        ///<summary>
        /// Gets product prices
        ///</summary>
        ///<return>List<Product></return>
        public List<Product> GetCartProductDetails(List<ProductQunatity> products)
        {
            List<Product> product = new List<Product>();
            foreach (ProductQunatity item in products)
            {
                Product product2 = _productContext.Product.Where(find => find.Id == item.Id && find.IsActive).FirstOrDefault(); 

                if(product2 == null )
                {
                    Product product1 = new Product()
                    {
                        Id=item.Id,
                        Name = null,
                    };
                    product.Add(product1);
                }
                else
                {
                    product.Add(product2);
                }
            }
            return product;
        }
        public List<Product> GetWishListProductDetails(List<Guid> productIds)
        {
            List<Product> products = new List<Product>();
            foreach (Guid item in productIds)
            {
                Product product = _productContext.Product.Where(find => find.Id == item && find.IsActive).FirstOrDefault();
                if(product == null)
                {
                    Product product1 = new Product();
                    product1.Id = item;
                    products.Add(product);
                }
                else
                {
                    products.Add(product);
                }
            }
            return products;
        }
        public Catalog GetCatalogDetails(Guid id)
        {

            Catalog result = _productContext.Catalog.Include(src => src.Category)
                .ThenInclude(src => src.Products)
                .Where(find => find.Id == id && find.IsActive).FirstOrDefault();
            return result;
        }
        public List<CategoryNamesResponseDTO> GetCategoryNamesList()
        {
            return _productContext.Category.Where(sel => sel.IsActive).Select(sel => new CategoryNamesResponseDTO {Id=sel.Id,Name=sel.Name }).ToList();
        }
    }
}
