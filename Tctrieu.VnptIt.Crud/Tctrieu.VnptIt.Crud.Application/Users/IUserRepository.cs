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
    /// <summary>
    /// Truy xuất dữ liệu User, cài đặt ở Infrastructure (MySQL)
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Kiểm tra email đã tồn tại, bỏ qua user có id = excludeId (dùng khi cập nhật)
        /// </summary>
        Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default);

        /// <summary>
        /// Lấy danh sách có lọc và phân trang, trả về kèm tổng số bản ghi
        /// </summary>
        Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
            GetUsersQuery query,
            CancellationToken ct = default);

        Task AddAsync(User user, CancellationToken ct = default);

        void Remove(User user);
    }
}
