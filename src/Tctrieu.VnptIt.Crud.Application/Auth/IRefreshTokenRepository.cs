using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Application.Auth
{
    /// <summary>
    /// Truy xuất dữ liệu RefreshToken, cài đặt ở Infrastructure (MySQL)
    /// </summary>
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);

        Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default);

        /// <summary>
        /// Thu hồi toàn bộ refresh token còn hiệu lực của user
        /// </summary>
        Task RevokeAllActiveAsync(int userId, CancellationToken ct = default);
    }
}
