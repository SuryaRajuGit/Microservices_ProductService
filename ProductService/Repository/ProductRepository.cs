using ProductService.Contracts;
using ProductService.Entity.Dto;
using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _productContext;

        public ProductRepository(ProductContext productContext)
        {
            _productContext = productContext;
        }
        public string IsProductExists(List<string> names)
        {
            var li = _productContext.Product.Select(item => item.Name).ToList();
            foreach (string item in names)
            {
                if (li.Contains(item))
                {
                    return item;
                }
            }
            return null;
        }

        public Guid? IsCatelogIdexists(ProductDTO productDTOs)
        {
            var l2 = _productContext.Category.Select(item => item.Id).ToList();
            if (!l2.Contains(productDTOs.CategoryId))
            {
                return productDTOs.CategoryId;
            }
            return null;
        }

        public void SaveProduct(List<Product> product)
        {
            _productContext.Product.AddRange(product);
            _productContext.SaveChanges();
        }

        public Tuple<string, Guid> IsProductExists(Guid id, Guid categoryId)
        {
            var x = _productContext.Category.Include(find => find.Products).Where(find => find.Id == categoryId).FirstOrDefault();
            if (x == null)
            {
                return new Tuple<string, Guid>("catalog", categoryId);
            }
            var v = x.Products.Any(find => find.Id == id && find.Visibility == true);
            if (!v)
            {
                return new Tuple<string, Guid>("product", id);
            }
            return new Tuple<string, Guid>(string.Empty, Guid.Empty); 
        }

        public Product GetProduct(Guid id, Guid categoryId)
        {
            Category x = _productContext.Category.Include(inc => inc.Products).Where(find => find.Id == categoryId).FirstOrDefault();
            Product y = x.Products.Where(item => item.Id == id && item.Visibility == true).FirstOrDefault();
            
            return y;
        }
        public int GetProductQunatity(Guid item)
        {
            var co = _productContext.Product.Where(find => find.Id == item && find.Visibility == true).FirstOrDefault(); 
            if(co == null)
            {
                return -1;
            }
            return co.Quantity;
        }
        public bool UpdateProduct(Product product,Guid CategoryId)
        {
            var c = _productContext.Category.Include(inc => inc.Products).Where(find => find.Id == product.CategoryId).Any(find => find.Id == product.CategoryId);
            if (!c)
            {
                return false;
            }
            var cx = _productContext.Product.FirstOrDefault(f => f.Id == product.Id);
            cx.Name = product.Name;
            cx.Price = 70;
            cx.Quantity = product.Quantity;
            cx.Visibility = product.Visibility;
            cx.Asset = null;
            
            _productContext.SaveChanges();
            return true;
        }

        public bool IsProductNameExists(UpdateProductDTO product)
        {
            return _productContext.Category.Include(inc => inc.Products).Where(find => find.Id == product.CategoryId).Any(find => find.Name == product.Name);

        }

        public Tuple<string,string> IsExists(CatalogDTO catalogDTO)
        {
            var catalog = _productContext.Catalog.Any(find => find.Name == catalogDTO.CatalogName);
            if(catalog)
            {
                return new Tuple<string, string>("catalog",catalogDTO.CatalogName);
            }
            var l = _productContext.Category.Select(item => item.Name).ToList();
            foreach (var item in catalogDTO.Category)
            {
                if(l.Contains(item.Name.ToString()))
                {
                    return new Tuple<string, string>("category", item.Name.ToString());
                }
            }
            return new Tuple<string, string>(string.Empty, string.Empty); 
        }
        public void SaveCatalog(Catalog catalogs)
        {
            _productContext.Catalog.Add(catalogs);
            _productContext.SaveChanges();
        }
        public bool UpdateProductQuantity(ProductToCartDTO updatePurchasedProduct)
        {
            var l = _productContext.Category.Include(term => term.Products).Where(src => src.Id == updatePurchasedProduct.CategoryId)
                .FirstOrDefault();
            var k = l.Products.Where(find => find.Id == updatePurchasedProduct.ProductId).FirstOrDefault();
            if(k == null)
            {
                return false;
            }
            k.Quantity = k.Quantity - updatePurchasedProduct.Quantity;
            _productContext.Product.Update(k);
            _productContext.SaveChanges();
            return true;
        }

        public int? CheckProductQuantity(ProductToCartDTO item)
        {
            var t = _productContext.Category.Include(term => term.Products).Where(find => find.Id == item.CategoryId).First();
            var y = t.Products.Where(find => find.Id == item.ProductId).First();
            if(y.Quantity < item.Quantity)
            {
                return y.Quantity;
            }
            return null;
               
        }
        public bool CheckProduct(Guid id)
        {
            return _productContext.Product.Any(find => find.Visibility == true && find.Id == id);
        }
        public Tuple<string,string> GetProductCount(Guid id,Guid categoryId)
        {
            //  Product product = _productContext.Category.Where(find => find.Id == categoryId).Select(term => term.Products.Where(fid => fid.Id == id).FirstOrDefault()).FirstOrDefault();
            var x = _productContext.Category.Include(term => term.Products).Where(find => find.Id == categoryId).FirstOrDefault();
            if (x == null)
            {
                return new Tuple<string, string>("category", categoryId.ToString());
            }
            var v = x.Products.Where(find => find.Id == id && find.Visibility == true).FirstOrDefault();
            if (v == null) 
            {
                return new Tuple<string, string>("product", id.ToString());
            }
            return new Tuple<string, string>(v.Price.ToString(), v.Quantity.ToString());

        }

        public bool IsProductExist(Guid id)
        {
            return _productContext.Product.Any(find => find.Id == id);
        }

        public Product Product(Guid id)
        {
            return _productContext.Product.First(find => find.Id == id);
        }

        public Tuple<string, string> IsCategoryExists(Guid catalogId, Guid categoryId)
        {
            var x = _productContext.Catalog.Include(term => term.Category).Where(find => find.Id == catalogId).FirstOrDefault();
            if(x == null)
            {
                return new Tuple<string, string>("catalog",catalogId.ToString());
            }
            var v = x.Category.Any(find => find.Id == categoryId);
            if(!v)
            {
                return new Tuple<string, string>("category", categoryId.ToString());
            }
            return new Tuple<string, string>(string.Empty, string.Empty); 
        }
        public List<Product> GetProductList(string name)
        {
            return _productContext.Product.Where(item => item.Name.ToLower().Contains(name.ToLower())).Where(term => term.Visibility == true)
              
              .ToList();

        }
        public List<Product> GetProductsList(Guid catalogId, Guid categoryId, int size, int pageNo)
        {
            var x = _productContext.Catalog.Include(s => s.Category).ThenInclude(s => s.Products).Where(find => find.Id == catalogId).First();
            var t = x.Category.Where(find => find.Id == categoryId).First();
            var p = t.Products;
            p.Select(find => find.Category = null);
            
            
            //.Where(f => f.Category.Select(f => f.Id).First() == categoryId).SelectMany(f => f.Category.SelectMany(s => s.Products)).Where(s => s.Visibility == true);
            var y = p.Skip((pageNo - 1) * 5)
              .Take(size).ToList();
            return y;
        }
        public float GetProductPrice(Guid id)
        {
            return _productContext.Product.Where(find => find.Id == id).Select(sel => sel.Price).First();
        }
        public bool DeleteProduct(Guid id)
        {
            var x = _productContext.Product.Where(find => find.Id == id).FirstOrDefault();
            if(x == null)
            {
                return false;
            }
            _productContext.Product.Remove(x);
            _productContext.SaveChanges();
            return true;
        }
        public ProductPrice GetProductsPrice(Guid id,int quantity)
        {
            var p = _productContext.Product.Where(find => find.Id == id).First();
            
            ProductPrice productPrice = new ProductPrice()
            {
                Id=p.Id,
                Price=p.Price
            };
            p.Quantity = p.Quantity - quantity;
            _productContext.Product.Update(p);
            _productContext.SaveChanges();
            return productPrice;
        }
        public List<Product> GetCartProductDetails(List<ProductQunatity> products)
        {
            List<Product> product = new List<Product>();
            foreach (var item in products)
            {
                var t = _productContext.Product.Where(find => find.Id == item.Id).FirstOrDefault(); 

                if(t == null )
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
                    product.Add(t);
                }
            }
            return product;
        }
        public List<Product> GetWishListProductDetails(List<Guid> productIds)
        {
            List<Product> products = new List<Product>();
            foreach (var item in productIds)
            {
                var x = _productContext.Product.Where(find => find.Id == item).FirstOrDefault();
                if(x == null)
                {
                    Product product = new Product();
                    product.Id = item;
                    products.Add(product);
                }
                else
                {
                    products.Add(x);
                }
            }
            return products;
        }
    }
}
