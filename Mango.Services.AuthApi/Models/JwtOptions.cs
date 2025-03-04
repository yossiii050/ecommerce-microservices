namespace Mango.Services.AuthApi.Models
{
    public class JwtOptions
    {
        public string Secret { get; set; }=string.Empty;
        public string Isuser { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
