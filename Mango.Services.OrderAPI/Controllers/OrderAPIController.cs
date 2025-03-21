using AutoMapper;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Mango.Services.OrderAPI.Utillity;
using Mango.Services.OrderAPI.Models;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        private IProductService _productService;


        public OrderAPIController(AppDbContext db, IMapper mapper, IProductService productService)
        {
            _db = db;
            _mapper = mapper;
            this._response = new ResponseDto();
            _productService = productService;
            
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto=_mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.orderTime=DateTime.Now;
                orderHeaderDto.Status=SD.Status_Pending;
                orderHeaderDto.OrderDetails=_mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated= _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId=orderCreated.OrderHeaderId;

                _response.Result=orderHeaderDto;
            }
            catch (Exception ex)
            {

                _response.IsSuccess=false;
                _response.Message=ex.Message;
            }

            return _response;

        }


        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {

                var options = new SessionCreateOptions
                {
                    SuccessUrl =stripeRequestDto.ApproveUrl,
                    CancelUrl =stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };


                foreach(var item in stripeRequestDto.orderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount=(long)(item.Price*100),
                            Currency="ils",
                            ProductData=new SessionLineItemPriceDataProductDataOptions
                            {
                                Name=item.Product.Name
                            }

                        },
                        Quantity=item.Count
                    };

                    options.LineItems.Add(sessionLineItem);
                }


                var service = new SessionService();
                Session session=service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId==stripeRequestDto.orderHeader.OrderHeaderId);
                orderHeader.StripeSessionId=session.Id;
                _db.SaveChanges();

                _response.Result=stripeRequestDto;
            }
            catch (Exception ex)
            {

                _response.Message=ex.Message;
                _response.IsSuccess=false;
            }
            return _response;
        }
    }
}
