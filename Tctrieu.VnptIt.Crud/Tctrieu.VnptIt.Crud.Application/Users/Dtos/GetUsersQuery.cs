using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Domain.Enums;

namespace Tctrieu.VnptIt.Crud.Application.Users.Dtos
{
    /// <summary>
    /// Điều kiện lọc và phân trang danh sách user
    /// </summary>
    public class GetUsersQuery
    {
        /// <summary>
        /// Số bản ghi mặc định mỗi trang
        /// </summary>
        public const int DEFAULT_PAGE_SIZE = 20;

        /// <summary>
        /// Số bản ghi tối đa mỗi trang
        /// </summary>
        public const int MAX_PAGE_SIZE = 100;

        public string? Keyword { get; set; }
        public UserStatusEnum? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = DEFAULT_PAGE_SIZE;
    }
}
