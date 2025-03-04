using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthApi.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
    }
}
