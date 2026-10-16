using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tctrieu.VnptIt.Crud.Application.Auth.Dtos
{
    // Dữ liệu đăng nhập
    public class LoginRequest
    {
        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = default!;

        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; } = default!;
    }
}
