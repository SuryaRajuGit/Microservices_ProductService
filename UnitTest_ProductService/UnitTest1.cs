using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductService.Controllers;
using ProductService.Entity.Dto;
using ProductService.Entity.Models;
using ProductService.Helpers;
using ProductService.Repository;
using ProductService.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

namespace UnitTest_ProductService
{
    public class UnitTest1
    {
        private readonly IMapper _mapper;
        private readonly ProductController _productController;
        private readonly ILogger _logger;
        private ProductContext _context;
        private ProductRepository _repository;
        private ProductServices _productService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public HttpClient CreateClient(string name)
        {
            return _httpClientFactory.CreateClient(name);
        }

        public UnitTest1()
        {

            IHostBuilder hostBuilder = Host.CreateDefaultBuilder().
            ConfigureLogging((builderContext, loggingBuilder) =>
            {
                loggingBuilder.AddConsole((options) =>
                {
                    options.IncludeScopes = true; //AddAuthentication
                });
            });
            IHost host = hostBuilder.Build();
            ILogger<ProductController> _logger = host.Services.GetRequiredService<ILogger<ProductController>>();

            MapperConfiguration mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Mappers());
            });


            // need to have access to the context
            Claim claim = new Claim("role", "Admin");
            Claim claim1 = new Claim("Id", "8d0c1df7-a887-4453-8af3-799e4a7ed013");
            ClaimsIdentity identity = new ClaimsIdentity(new[] { claim, claim1 }, "BasicAuthentication"); // this uses basic auth
            ClaimsPrincipal contextUser = new ClaimsPrincipal(identity); //add claims as needed

            //...then set user and other required properties on the httpContext as needed
            DefaultHttpContext httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };

            //Controller needs a controller context to access HttpContext
            HttpContextAccessor _httpContextAccessor = new HttpContextAccessor()
            {
                HttpContext = httpContext
            };


            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;
            DbContextOptions<ProductContext> options = new DbContextOptionsBuilder<ProductContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _context = new ProductContext(options);

            _repository = new ProductRepository(_context);

            _productService = new ProductServices(_repository, _mapper,_httpContextAccessor);

            _productController = new ProductController(_productService, _logger);
            AddData();
            _context.Database.EnsureCreated();
        }
        public void AddData()
        {
            string path = @"C:\Users\Hp\source\repos\ProductService\ProductService\Entity\Files\Products.csv";
            string ReadCSV = File.ReadAllText(path);
            string[] data = ReadCSV.Split('\r');
            foreach (string item in data)
            {
                string[] row = item.Split(",");
                if (row[0] == "catalog_name")
                {
                    continue;
                }
                Catalog catalogs = new Catalog { Id = Guid.Parse(row[1]), Name = item[0].ToString(),Category= new Collection<Category>() };
                Category categories = new Category { Id = Guid.Parse(row[3]), CatalogId = Guid.Parse(row[1]), Name = item[2].ToString(),Products=new List<Product>() };
                Product product = new Product {Id=Guid.Parse(row[5]),Name=row[4],Description=row[6],Price=float.Parse(row[7]),Quantity=int.Parse(row[8]),Visibility=bool.Parse(row[9]),Asset=null,CategoryId=Guid.Parse(row[3]), };
                categories.Products.Add(product);
                catalogs.Category.Add(categories);
                _context.Catalog.Add(catalogs);
                _context.SaveChanges();
            }
        }
        [Fact]
        public void GetCatalog_Test()
        {
            ActionResult response = _productController.GetCatalog(Guid.Parse("56ef95d7-2096-4a88-b0f7-82f4dbb6cc35"));
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void AddProduct_Test()
        {
            ProductDTO productDTO = new ProductDTO()
            {
                CategoryId = Guid.Parse("4944226f-36a7-445f-a9e5-d5c2ba1f525f"),
                Product = new List<CategoryDTO>()
                
            };
            CategoryDTO categoryDTO = new CategoryDTO()
            {
                Asset = null,
                Name = "test",
                Description = "Test",
                Price = 10,
                Quantity = 10,
                Visibility = true
            };
            ActionResult response = _productController.AddProduct(productDTO).Result;
            ObjectResult result = Assert.IsType<ObjectResult>(response);
            Assert.Equal(201, result.StatusCode);
        }

        [Fact]
        public void GetProduct_Test()
        {
            ActionResult response = _productController.GetProduct(Guid.Parse("2a52169a-e58f-42e8-bc0e-4603c361c589"),
                Guid.Parse("4944226f-36a7-445f-a9e5-d5c2ba1f525f")).Result;
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }

        //[Fact]
        //public void UpdateProductDetails_Test()
        //{
        //    UpdateProductDTO product = new UpdateProductDTO()
        //    {
        //        CategoryId=Guid.Parse("4944226f-36a7-445f-a9e5-d5c2ba1f525f"),
        //        Name="Test ",
        //        ProductId= Guid.Parse("2a52169a-e58f-42e8-bc0e-4603c361c589")
        //        ,Asset=null,Description="ss",Price=10,Quantity=10,Visibility=true

        //    };
        //    IActionResult response = _productController.UpdateProductDetails(product);
        //    OkObjectResult result = Assert.IsType<OkObjectResult>(response);
        //    Assert.Equal(200, result.StatusCode);
        //}

        [Fact]
        public void CreateCatalog_Test()
        {
            CatalogDTO catalogDTO = new CatalogDTO()
            {
                CatalogName = "Catalog",
                Category = new List<CategoryDTOcatalog>()
                {
                    new CategoryDTOcatalog()
                    {
                        Name="Test product",
                        Products = new List<CategoryDTO>()
                        {
                            new CategoryDTO()
                            {
                                Name="Test",
                                Asset=null,
                                Description="test",
                                Price=10,
                                Quantity=10,
                                Visibility=true
                            }
                        }
                    }
                }
            };
            ActionResult response = _productController.CreateCatalog(catalogDTO).Result;
            ObjectResult result = Assert.IsType<ObjectResult>(response);
            Assert.Equal(201, result.StatusCode);
        }

        [Fact]
        public void IsProductExistsInventory_Test()
        {
            string response = _productController.IsProductExistsInventory(Guid.Parse("2a52169a-e58f-42e8-bc0e-4603c361c589"),
                Guid.Parse("4944226f-36a7-445f-a9e5-d5c2ba1f525f"));
            Assert.IsType<string>(response);
        
        }

        [Fact]
        public void UpdateProductQuantity_Test()
        {
            ProductToCartDTO productToCartDTO = new ProductToCartDTO()
            { 
                Quantity=1,
                ProductId= Guid.Parse("2a52169a-e58f-42e8-bc0e-4603c361c589"),
                CategoryId= Guid.Parse("4944226f-36a7-445f-a9e5-d5c2ba1f525f")
            };

            IActionResult response = _productController.UpdateProductQuantity(productToCartDTO);
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void IsCartproductsExist_Test()
        {
            List<Guid> list = new List<Guid>();
            list.Add(Guid.Parse("2a52169a-e58f-42e8-bc0e-4603c361c589"));
            IActionResult response = _productController.IsCartproductsExist(list);
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void CartProductsQuantity_Test()
        {
            List<ProductQunatity> productQunatities = new List<ProductQunatity>();
            
            ProductQunatity productQunatity = new ProductQunatity()
            {
                Quantity=2,Id= Guid.Parse("2a52169a-e58f-42e8-bc0e-4603c361c589")
            };
            productQunatities.Add(productQunatity);
            IActionResult response = _productController.CartProductsQuantity(productQunatities);
            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void GetProductList_Test()
        {
            IActionResult response = _productController.GetProductList(Guid.Parse("4944226f-36a7-445f-a9e5-d5c2ba1f525f"),
                 Guid.Parse("56ef95d7-2096-4a88-b0f7-82f4dbb6cc35"),1,1,"Price", "ASC");

            OkObjectResult result = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public void GetProductPrice_Test()
        {
            float response = _productController.GetProductPrice(Guid.Parse("2a52169a-e58f-42e8-bc0e-4603c361c589"));
            float result = Assert.IsType<float>(response);
            Assert.Equal(10, result);
        }
        //[Fact]
        //public void GetWishListProductDetails_Test()
        //{
        //    List<Guid> guids = new List<Guid>();
        //    guids.Add(Guid.Parse("2a52169a-e58f-42e8-bc0e-4603c361c589"));
        //    CartResponseDTO response = _productController.GetWishListProductDetails(guids)[0];
        //    CartResponseDTO result = Assert.IsType<CartResponseDTO>(x);
        //    Assert.Equal("potatos", result.Name);
        //}
       
    }
}
