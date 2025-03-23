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
using Mango.MessageBus;
using Microsoft.EntityFrameworkCore;

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
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public OrderAPIController(AppDbContext db, IMapper mapper, IProductService productService, IMessageBus messageBus, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            this._response = new ResponseDto();
            _productService = productService;
            _messageBus= messageBus;
            _configuration=configuration;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDto? Get(string? userId="")
        {
            try
            {
                IEnumerable<OrderHeader> objList;
                if(User.IsInRole(SD.RoleAdmin))
                {
                    objList = _db.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToList();

                }
                else
                {
                    objList = _db.OrderHeaders.Include(u => u.OrderDetails).Where(u=>u.UserID==userId).OrderByDescending(u => u.OrderHeaderId).ToList();

                }
                _response.Result=_mapper.Map<IEnumerable<OrderHeaderDto>>(objList);
            }
            catch (Exception ex)
            {

                _response.IsSuccess=false;
                _response.Message=ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDto? Get(int id)
        {
            try
            {
                OrderHeader orderHeader= _db.OrderHeaders.Include(u=>u.OrderDetails).First(u=>u.OrderHeaderId==id);
                _response.Result=_mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {

                _response.IsSuccess=false;
                _response.Message=ex.Message;
            }
            return _response;
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

                var DiscountsObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions()
                    {
                        Coupon=stripeRequestDto.orderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDto.orderHeader.OrderDetails)
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

                if (stripeRequestDto.orderHeader.Discount>0)
                {
                    options.Discounts=DiscountsObj;
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

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId==orderHeaderId);


                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentUntentService=new PaymentIntentService();
                PaymentIntent paymentIntent = paymentUntentService.Get(session.PaymentIntentId);
                
                if(paymentIntent.Status=="succeeded") 
                {
                    //payment successful
                    orderHeader.PaymentIntentId=paymentIntent.Id;
                    orderHeader.Status=SD.Status_Approved;
                    _db.SaveChanges();
                    RewardsDto rewardsDto = new()
                    {
                        OrderId=orderHeader.OrderHeaderId,
                        RewardsActivity=Convert.ToInt32(orderHeader.OrderTotal),
                        UserId=orderHeader.UserID,
                    };

                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublicMessage(rewardsDto, topicName);
                    
                    _response.Result= _mapper.Map<OrderHeaderDto>(orderHeader);

                }
                
                
            }
            catch (Exception ex)
            {

                _response.Message=ex.Message;
                _response.IsSuccess=false;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader= _db.OrderHeaders.First(u=>u.OrderHeaderId==orderId);
                if (orderHeader!=null)
                {
                    if(newStatus==SD.Status_Cancelled)
                    {
                        //Refund
                        var options = new RefundCreateOptions
                        {
                            Reason=RefundReasons.RequestedByCustomer,
                            PaymentIntent=orderHeader.PaymentIntentId
                        };

                        var service = new RefundService();
                        Refund refund=service.Create(options);
                        orderHeader.Status=newStatus;
                    }
                    orderHeader.Status=newStatus;
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                _response.IsSuccess=false;
            }
            return _response;
        }
    }
}
