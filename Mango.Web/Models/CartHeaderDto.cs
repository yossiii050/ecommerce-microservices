﻿
namespace Mango.Web.Models.Dto
{
    public class CartHeaderDto
    {
        
        public int CartHeaderId { get; set; }
        public string? UserID { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }


    }
}

