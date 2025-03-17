
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.Dto
{
    public class CartHeaderDto
    {
        
        public int CartHeaderId { get; set; }
        public string? UserID { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }
        //for services bus
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Email { get; set; }

    }
}

