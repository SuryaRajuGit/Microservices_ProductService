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
            int c = 0;
            foreach (var item in data)
            {
                string[] row = item.Split(",");
                Catalog catalogs = new Catalog { Id = Guid.Parse(row[1]), Name = row[0].ToString(), IsActive = true };
                Category categories = new Category { Id = Guid.Parse(row[3]), CatalogId = Guid.Parse(row[1]), Name = row[2].ToString(), IsActive = true };
                Product products = new Product { CategoryId = Guid.Parse(row[3]), Name = row[4], Id = Guid.Parse(row[5]), Description = row[6], Price = float.Parse(row[7]), Quantity = int.Parse(row[8]), Asset = null, Visibility = true, IsActive = true };

                modelBuilder.Entity<Catalog>().HasData(catalogs);
                modelBuilder.Entity<Category>().HasData(categories);
                modelBuilder.Entity<Product>().HasData(products);
                if(1 == c + 1)
                {
                    break;
                }
            }
            modelBuilder.Entity<Catalog>()
                .HasMany(c => c.Category)
                .WithOne(ca => ca.Catalog)
                .HasForeignKey(ca => ca.CatalogId);

            modelBuilder.Entity<Category>()
                .HasMany(p => p.Products)
                .WithOne(pc => pc.Category)
                .HasForeignKey(pc => pc.CategoryId);
            
           
        }
    }
}
