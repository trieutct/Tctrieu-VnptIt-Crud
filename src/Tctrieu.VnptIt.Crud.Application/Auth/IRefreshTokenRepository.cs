using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Application.Auth
{
    // Truy xuất dữ liệu RefreshToken, cài đặt ở Infrastructure (MySQL)
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);

        Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default);

        // Thu hồi toàn bộ refresh token còn hiệu lực của user
        Task RevokeAllActiveAsync(int userId, CancellationToken ct = default);
    }
}
