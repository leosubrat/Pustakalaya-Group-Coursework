using Pustakalaya.DTOs;
using System.Threading.Tasks;

namespace Pustakalaya.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto model);
        Task<AuthResponseDto> LoginAsync(LoginDto model);
    }
}