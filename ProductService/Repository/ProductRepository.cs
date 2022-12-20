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
            if (!l2.Contains(productDTOs.Category_id))
            {
                return productDTOs.Category_id;
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
            bool lr = false;
            foreach (var item in _productContext.Category)
            {
                if (item.Id == id)
                {
                    lr = true;
                    bool ll = item.Products.Any(find => find.Id == id);
                    if (ll)
                    {
                        return null;
                    }
                }
            }
            if (!lr)
            {
                return new Tuple<string, Guid>("categoryId", id);
            }
            return new Tuple<string, Guid>("id", id);
        }

        public Product GetProduct(Guid id, Guid categoryId)
        {
            Category x = _productContext.Category.Include(inc => inc.Products).Where(find => find.Id == categoryId).FirstOrDefault();
            var y = x.Products.Where(f => f.Id == id).FirstOrDefault();
            return y;
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
              //  _productContext.Product.AddRange(item.Products);
            }
            return new Tuple<string, string>(string.Empty, string.Empty); ;
        }
        public Guid SaveCatalog(Catalog catalogs)
        {
            _productContext.Catalog.Add(catalogs);
            return catalogs.Id;
        }

        public int? GetProductCount(Guid id,Guid categoryId)
        {
            Product product = _productContext.Category.Where(find => find.Id == categoryId).Select(term => term.Products.Where(fid => fid.Id == id).FirstOrDefault()).FirstOrDefault();
            if(product == null)
            {
                return null;
            }
            return product.Quantity;
        }
    }
}
