using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Auth.Dtos;

namespace Tctrieu.VnptIt.Crud.Application.Auth
{
    // Các nghiệp vụ xác thực
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
        Task LogoutAsync(string refreshToken, CancellationToken ct = default);
    }
}
