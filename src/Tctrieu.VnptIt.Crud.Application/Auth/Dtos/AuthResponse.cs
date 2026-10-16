using System;
using System.Text.Json.Serialization;
using Tctrieu.VnptIt.Crud.Application.Users.Dtos;

namespace Tctrieu.VnptIt.Crud.Application.Auth.Dtos
{
    /// <summary>
    /// Kết quả đăng nhập / đăng ký / làm mới token
    /// </summary>
    public class AuthResponse
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "Bearer";
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = default!;
        [JsonPropertyName("access_token_expires_at")]
        public DateTime AccessTokenExpiresAt { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = default!;
        [JsonPropertyName("refresh_token_expires_at")]
        public DateTime RefreshTokenExpiresAt { get; set; }
        [JsonPropertyName("user")]
        public UserDto User { get; set; } = default!;
    }
}
