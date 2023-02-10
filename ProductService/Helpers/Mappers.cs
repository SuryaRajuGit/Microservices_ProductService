using AutoMapper;
using ProductService.Entity.Dto;
using ProductService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Helpers
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<Product, CategoryDTO>().ReverseMap()
               
                ;


            CreateMap<ProductResponseDTO, Product>().ReverseMap();
            CreateMap<CartResponseDTO,Product>().ReverseMap();
            CreateMap<UpdateProductDTO,Product >().ReverseMap();

            CreateMap<ProductDetailsDto, Product>().ReverseMap();
        }
        
    }
}
