namespace Mango.Web.Service.IService
{
    public interface ITokenProvider
    {
        void setToken(string token);
        string? getToken();
        void ClearToken();
    }
}
