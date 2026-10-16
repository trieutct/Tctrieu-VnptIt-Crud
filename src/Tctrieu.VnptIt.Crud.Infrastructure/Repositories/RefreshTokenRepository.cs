using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Auth;
using Tctrieu.VnptIt.Crud.Domain.Entities;
using Tctrieu.VnptIt.Crud.Infrastructure.Persistence;

namespace Tctrieu.VnptIt.Crud.Infrastructure.Repositories
{
    /// <summary>
    /// Truy xuất bảng refresh_tokens trên MySQL
    /// </summary>
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _db;

        public RefreshTokenRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        {
            return _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct)!;
        }

        public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
        {
            await _db.RefreshTokens.AddAsync(refreshToken, ct);
        }

        public async Task RevokeAllActiveAsync(int userId, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var active_tokens = await _db.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > now)
                .ToListAsync(ct);

            foreach (var token in active_tokens)
            {
                token.Revoke();
            }
        }
    }
}
