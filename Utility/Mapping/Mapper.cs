using AutoMapper;
using Models;
using Models.FormModel;
using Models.ProductFilterOptions;


namespace ControllersServices.ProductManagement
{
    public class ProductFormToProductMapper : Profile
    {
        public ProductFormToProductMapper()
        {
            CreateMap<ProductFormModel, Product>();
            CreateMap<ProductFilterOptionsRequest, ProductFilterOptionsQuery>()
            .ForMember(dest => dest.CategoriesFilteredIds, opt => opt.Ignore());
        }
    }
}