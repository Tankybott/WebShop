using Models.DatabaseRelatedModels;

namespace Services.OrderServices.Interfaces
{
    public interface IOrderTableHTMLBuilder
    {
        string BuildHtml(OrderHeader order);
    }
}