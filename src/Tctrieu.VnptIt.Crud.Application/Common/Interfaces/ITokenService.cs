using System;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Application.Common.Interfaces
{
    // Sinh access token (JWT) và refresh token
    public interface ITokenService
    {
        // Sinh JWT access token cho user, trả về kèm thời điểm hết hạn (UTC)
        (string Token, DateTime ExpiresAt) CreateAccessToken(User user);

        // Sinh refresh token ngẫu nhiên, trả về kèm thời điểm hết hạn (UTC)
        (string Token, DateTime ExpiresAt) CreateRefreshToken();

        // Băm refresh token để lưu/tra cứu trong DB
        string HashRefreshToken(string refreshToken);
    }
}
