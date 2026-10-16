using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Users;
using Tctrieu.VnptIt.Crud.Application.Users.Dtos;
using Tctrieu.VnptIt.Crud.Domain.Entities;
using Tctrieu.VnptIt.Crud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Tctrieu.VnptIt.Crud.Infrastructure.Repositories
{
    /// <summary>
    /// Truy xuất bảng users trên MySQL
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct)!;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var normalized_email = email.Trim().ToLowerInvariant();

            return _db.Users.FirstOrDefaultAsync(x => x.Email == normalized_email, ct)!;
        }

        public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default)
        {
            var normalized_email = email.Trim().ToLowerInvariant();

            return _db.Users.AnyAsync(
                x => x.Email == normalized_email && (excludeId == null || x.Id != excludeId),
                ct);
        }

        public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
            GetUsersQuery query,
            CancellationToken ct = default)
        {
            // Chỉ đọc nên không cần EF theo dõi thay đổi
            var users = _db.Users.AsNoTracking().AsQueryable();

            // Lọc theo từ khóa: họ tên, email, số điện thoại
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.Trim();
                users = users.Where(x =>
                    x.FullName.Contains(keyword) ||
                    x.Email.Contains(keyword) ||
                    (x.PhoneNumber != null && x.PhoneNumber.Contains(keyword)));
            }

            // Lọc theo trạng thái
            if (query.Status.HasValue)
            {
                users = users.Where(x => x.Status == query.Status.Value);
            }

            var total_count = await users.CountAsync(ct);

            // Mới nhất lên đầu, sau đó phân trang
            var items = await users
                .OrderByDescending(x => x.Id)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(ct);

            return (items, total_count);
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _db.Users.AddAsync(user, ct);
        }

        public void Remove(User user)
        {
            _db.Users.Remove(user);
        }
    }
}
