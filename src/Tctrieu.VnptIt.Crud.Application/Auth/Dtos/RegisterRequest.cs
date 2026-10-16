using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tctrieu.VnptIt.Crud.Application.Auth.Dtos
{
    /// <summary>
    /// Dữ liệu đăng ký tài khoản
    /// </summary>
    public class RegisterRequest
    {
        [JsonPropertyName("full_name")]
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string FullName { get; set; } = default!;

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(150, ErrorMessage = "Email tối đa 150 ký tự")]
        public string Email { get; set; } = default!;

        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string Password { get; set; } = default!;

        [JsonPropertyName("phone_number")]
        [MaxLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
        public string? PhoneNumber { get; set; }
    }
}
