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
        public Guid? IsCatelogIdexists(ProductDTO productDTOs)
        {
            List<Guid> catalogIds = _productContext.Category.Select(item => item.Id).ToList();
            if (!catalogIds.Contains(productDTOs.CategoryId))
            {
                return productDTOs.CategoryId;
            }
            return null;
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
        public Tuple<string, Guid> IsProductExists(Guid id, Guid categoryId)
        {
            Category category = _productContext.Category.Include(find => find.Products).Where(find => find.Id == categoryId).FirstOrDefault();
            if (category == null)
            {
                return new Tuple<string, Guid>(Constants.Catalog, categoryId);
            }
            bool isProductExist = category.Products.Any(find => find.Id == id && find.Visibility );
            if (!isProductExist)
            {
                return new Tuple<string, Guid>(Constants.Product, id);
            }
            return new Tuple<string, Guid>(string.Empty, Guid.Empty); 
        }

        ///<summary>
        /// Gets Product details
        ///</summary>
        ///<return>Product</return>
        public Product GetProduct(Guid id, Guid categoryId)
        {
            Product productInDb = (from cat in _productContext.Category
                         where cat.Id == categoryId && cat.IsActive
                         from prod in cat.Products
                         where prod.Id == id && prod.IsActive && prod.Visibility
                         select prod).FirstOrDefault() ;
            return productInDb;
        }
        ///<summary>
        /// Gets Product quantity
        ///</summary>
        ///<return>int</return>
        public int GetProductQunatity(Guid item)
        {
            Product product = _productContext.Product.Where(find => find.Id == item && find.Visibility).FirstOrDefault(); 
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
        public bool UpdateProduct(UpdateProductDTO productDto,Guid CategoryId)
        {
            bool isProductExist = _productContext.Category.Include(inc => inc.Products).Where(find => find.Id == productDto.CategoryId).Any(find => find.Id == productDto.CategoryId);
            if (!isProductExist)
            {
                return false;
            }
            Product productInDb = _productContext.Product.FirstOrDefault(find => find.Id == productDto.Id);
            var properties =  typeof(UpdateProductDTO).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
                object propertyValue = property.GetValue(productDto);

                if(propertyValue != null)
                {
                    productInDb.GetType().GetProperty(propertyName).SetValue(productInDb, propertyValue);
                }
            }
            productInDb.UpdateDate = DateTime.Now;
            _productContext.SaveChanges();
            return true;
        }

        ///<summary>
        /// Gets catalog details
        ///</summary>
        ///<return>bool</return>
        public bool IsProductNameExists(UpdateProductDTO product)
        {
            return _productContext.Category.Include(inc => inc.Products).Where(find => find.Id == product.CategoryId).Any(find => find.Name == product.Name);
        }

        ///<summary>
        /// Checks if the catalog exits or not
        ///</summary>
        ///<return>Tuple<string,string></return>
        public Tuple<string,string> IsExists(CatalogDTO catalogDTO)
        {
            bool isCatalogExist = _productContext.Catalog.Any(find => find.Name == catalogDTO.CatalogName);
            if(isCatalogExist)
            {
                return new Tuple<string, string>(Constants.Catalog,catalogDTO.CatalogName);
            }
            List<string> categoryList = _productContext.Category.Select(item => item.Name).ToList();
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
        ///<summary>
        /// Updates product quantity of product in inventory
        ///</summary>
        ///<return>bool</return>
        public bool UpdateProductQuantity(ProductToCartDTO updatePurchasedProduct)
        {
            Category category = _productContext.Category.Include(term => term.Products).Where(src => src.Id == updatePurchasedProduct.CategoryId)
                .FirstOrDefault();
            Product product = category.Products.Where(find => find.Id == updatePurchasedProduct.ProductId).FirstOrDefault();
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
            Category category = _productContext.Category.Include(term => term.Products).Where(find => find.Id == item.CategoryId).First();
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
            return _productContext.Product.Any(find => find.Visibility == true && find.Id == id);
        }
        ///<summary>
        /// Gets product count 
        ///</summary>
        ///<return>Tuple<string,string></return>
        public Tuple<string,string> GetProductCount(Guid id,Guid categoryId)
        {
            //  Product product = _productContext.Category.Where(find => find.Id == categoryId).Select(term => term.Products.Where(fid => fid.Id == id).FirstOrDefault()).FirstOrDefault();
            Category category = _productContext.Category.Include(term => term.Products).Where(find => find.Id == categoryId).FirstOrDefault();
            if (category == null)
            {
                return new Tuple<string, string>(Constants.Category, categoryId.ToString());
            }
            Product product = category.Products.Where(find => find.Id == id && find.Visibility == true).FirstOrDefault();
            if (product == null) 
            {
                return new Tuple<string, string>(Constants.Product, id.ToString());
            }
            return new Tuple<string, string>(product.Price.ToString(), product.Quantity.ToString());

        }
        ///<summary>
        /// checks of the product exist or not 
        ///</summary>
        ///<return>bool</return>
        public bool IsProductExist(Guid id)
        {
            return _productContext.Product.Any(find => find.Id == id);
        }

        ///<summary>
        /// returns product details
        ///</summary>
        ///<return>Product</return>
        public Product Product(Guid id)
        {
            return _productContext.Product.First(find => find.Id == id);
        }

        ///<summary>
        /// Checks if the caregory exists or not
        ///</summary>
        ///<return>Tuple<string,string></return>
        public Tuple<string, string> IsCategoryExists(Guid catalogId, Guid categoryId)
        {
            Catalog catalog = _productContext.Catalog.Include(term => term.Category).Where(find => find.Id == catalogId).FirstOrDefault();
            if(catalog == null)
            {
                return new Tuple<string, string>(Constants.Catalog,catalogId.ToString());
            }
            bool isCategoryExist = catalog.Category.Any(find => find.Id == categoryId);
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
            return _productContext.Product.Where(item => item.Name.ToLower().Contains(name.ToLower())).Where(term => term.Visibility == true)
              .ToList();
        }
        ///<summary>
        /// Gets Producst List
        ///</summary>
        ///<return>List<Product></return>
        public List<Product> GetProductsList(Guid catalogId, Guid categoryId, int size, int pageNo)
        {
           List<Product> result = (from catalog1 in _productContext.Catalog
                          where catalog1.Id == catalogId && catalog1.IsActive
                          from category1 in catalog1.Category
                          where category1.IsActive && category1.Id == categoryId
                          from product in category1.Products
                          where product.IsActive && product.Visibility
                          select product).Skip((pageNo-1)*5).Take(size).ToList();


            Catalog catalog = _productContext.Catalog.Include(s => s.Category).ThenInclude(s => s.Products).Where(find => find.Id == catalogId).First();
            Category category = catalog.Category.Where(find => find.Id == categoryId).First();
            ICollection<Product> productList = category.Products;
            productList.Select(find => find.Category = null);
            //.Where(f => f.Category.Select(f => f.Id).First() == categoryId).SelectMany(f => f.Category.SelectMany(s => s.Products)).Where(s => s.Visibility == true);
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
            return _productContext.Product.Where(find => find.Id == id).Select(sel => sel.Price).First();
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
            Product product = _productContext.Product.Where(find => find.Id == id).First();
            
            ProductPrice productPrice = new ProductPrice()
            {
                Id= product.Id,
                Price= product.Price
            };
            product.Quantity = product.Quantity - quantity;
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
                Product product2 = _productContext.Product.Where(find => find.Id == item.Id).FirstOrDefault(); 

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
                Product product = _productContext.Product.Where(find => find.Id == item).FirstOrDefault();
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
            return _productContext.Catalog.Include(src => src.Category).ThenInclude(src => src.Products).Where(find => find.Id == id).FirstOrDefault();
        }

    }
}
