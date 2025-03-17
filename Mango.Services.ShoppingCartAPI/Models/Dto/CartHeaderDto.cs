
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCartAPI.Models.Dto
{
    public class CartHeaderDto
    {
        
        public int CartHeaderId { get; set; }
        public string? UserID { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }
        //for services bus
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }


    }
}

