using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tctrieu.VnptIt.Crud.Application.Users.Dtos
{
    /// <summary>
    /// Dữ liệu tạo user mới
    /// </summary>
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(150, ErrorMessage = "Email tối đa 150 ký tự")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string Password { get; set; } = default!;

        [MaxLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
        public string? PhoneNumber { get; set; }
    }
}
