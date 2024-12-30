using AutoMapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ControllersServices.ProductManagement
{
    public class ProductFormToProductMapper : Profile
    {
        public ProductFormToProductMapper()
        {
            CreateMap<ProductFormModel, Product>();
        }
    }
}