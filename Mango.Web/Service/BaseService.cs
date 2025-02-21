using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.Utillity.SD;
namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(IHttpClientFactory httpClientFactory)
        {

            _httpClientFactory=httpClientFactory;

        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("MangoApi");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");

                // TODO: add token support

                message.RequestUri=new Uri(requestDto.Url);
                if (requestDto.Data != null)
                {
                    message.Content= new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? apiResponse = null;

                switch (requestDto.ApiType)
                {
                    case ApiType.POST:
                        message.Method=HttpMethod.Post;
                        break;
                    case ApiType.DELETE:
                        message.Method=HttpMethod.Delete;
                        break;
                    case ApiType.PUT:
                        message.Method=HttpMethod.Put;
                        break;
                    default:
                        message.Method=HttpMethod.Get;
                        break;
                }

                apiResponse=await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess=false, Message="Not Found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess=false, Message="Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess=false, Message="Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess=false, Message="Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponeDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponeDto;
                }
            }catch(Exception ex)
            {
                var dto = new ResponseDto
                {
                    Message=ex.Message.ToString(),
                    IsSuccess=false
                };
                return dto;
            }
        }
    }
}
