using Mango.Services.AuthApi.Models.Dto;

namespace Mango.Services.AuthApi.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegisterationRequestDto registerationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);

        Task<bool> AssignRole(string email ,string roleName);
    }
}
