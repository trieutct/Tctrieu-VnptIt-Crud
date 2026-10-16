using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tctrieu.VnptIt.Crud.Application.Users.Dtos
{
    // Dữ liệu cập nhật user (không cho đổi email, mật khẩu ở đây)
    public class UpdateUserRequest
    {
        [JsonPropertyName("full_name")]
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string FullName { get; set; } = default!;

        [JsonPropertyName("phone_number")]
        [MaxLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
        public string? PhoneNumber { get; set; }
    }
}
