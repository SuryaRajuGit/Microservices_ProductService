using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Entity.Models
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {

        }

        public DbSet<Catalog> Catalog { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Product> Product { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string path = @"C:\Users\Hp\source\repos\ProductService\ProductService\Entity\Files\Products.csv";
            string ReadCSV = File.ReadAllText(path);
            var data = ReadCSV.Split('\r');
            foreach (var item in data)
            {
                Guid id = Guid.NewGuid();
                Catalog catalogs = new Catalog {Id=id,Name=item[0].ToString()};
                Category categories = new Category {Id=Guid.NewGuid(),CatalogId=id,Name=item[1].ToString() };

            }
        }
    }
}
