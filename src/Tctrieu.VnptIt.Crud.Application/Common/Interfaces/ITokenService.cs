using System;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Application.Common.Interfaces
{
    /// <summary>
    /// Sinh access token (JWT) và refresh token
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Sinh JWT access token cho user, trả về kèm thời điểm hết hạn (UTC)
        /// </summary>
        (string Token, DateTime ExpiresAt) CreateAccessToken(User user);

        /// <summary>
        /// Sinh refresh token ngẫu nhiên, trả về kèm thời điểm hết hạn (UTC)
        /// </summary>
        (string Token, DateTime ExpiresAt) CreateRefreshToken();

        /// <summary>
        /// Băm refresh token để lưu/tra cứu trong DB
        /// </summary>
        string HashRefreshToken(string refreshToken);
    }
}
