using Models.DatabaseRelatedModels;
using Services.EmailFactory.models;

namespace Services.EmailFactory.interfaces
{
    internal interface IOrderEmailBuilder: IEmailBuilder<OrderEmailInput>
    {
    }
}
