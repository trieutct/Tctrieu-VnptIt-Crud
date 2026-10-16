using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Users.Dtos;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Application.Users
{
    // Truy xuất dữ liệu User, cài đặt ở Infrastructure (MySQL)
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

        // Kiểm tra email đã tồn tại, bỏ qua user có id = excludeId (dùng khi cập nhật)
        Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default);

        // Lấy danh sách có lọc và phân trang, trả về kèm tổng số bản ghi
        Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
            GetUsersQuery query,
            CancellationToken ct = default);

        Task AddAsync(User user, CancellationToken ct = default);

        void Remove(User user);
    }
}
