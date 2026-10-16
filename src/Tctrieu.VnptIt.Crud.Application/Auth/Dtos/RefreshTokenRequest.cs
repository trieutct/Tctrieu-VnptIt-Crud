using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tctrieu.VnptIt.Crud.Application.Auth.Dtos
{
    /// <summary>
    /// Dữ liệu làm mới token / đăng xuất
    /// </summary>
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refresh_token")]
        [Required(ErrorMessage = "Refresh token là bắt buộc")]
        public string RefreshToken { get; set; } = default!;
    }
}
