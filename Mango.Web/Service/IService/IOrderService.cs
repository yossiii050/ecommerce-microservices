using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);

        Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto);

        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);

    }
}
