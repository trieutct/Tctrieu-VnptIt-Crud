using System;
using Tctrieu.VnptIt.Crud.Domain.Common;
using Tctrieu.VnptIt.Crud.Domain.Exceptions;

namespace Tctrieu.VnptIt.Crud.Domain.Entities
{
    /// <summary>
    /// Refresh token của user, chỉ lưu bản hash (không lưu token gốc)
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        public int UserId { get; private set; }
        public string TokenHash { get; private set; } = default!;
        public DateTime ExpiresAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        /// <summary>
        /// Hash của token mới thay thế token này (khi refresh)
        /// </summary>
        public string? ReplacedByTokenHash { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsActive => !IsRevoked && !IsExpired;

        // Constructor rỗng cho EF Core
        private RefreshToken()
        {
        }

        /// <summary>
        /// Tạo refresh token mới cho user
        /// </summary>
        public static RefreshToken Create(int userId, string tokenHash, DateTime expiresAt)
        {
            if (string.IsNullOrWhiteSpace(tokenHash))
                throw new DomainException("Refresh token không hợp lệ");

            return new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAt = expiresAt,
            };
        }

        /// <summary>
        /// Thu hồi token, kèm hash token thay thế nếu có
        /// </summary>
        public void Revoke(string? replacedByTokenHash = null)
        {
            if (IsRevoked) return;

            RevokedAt = DateTime.UtcNow;
            ReplacedByTokenHash = replacedByTokenHash;
            MarkUpdated();
        }
    }
}
