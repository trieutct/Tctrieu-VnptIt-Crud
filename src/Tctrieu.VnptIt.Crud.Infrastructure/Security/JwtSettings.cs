namespace Tctrieu.VnptIt.Crud.Infrastructure.Security
{
    /// <summary>
    /// Cấu hình JWT, đọc từ section "Jwt" trong appsettings.json
    /// </summary>
    public class JwtSettings
    {
        public const string SECTION_NAME = "Jwt";

        /// <summary>
        /// Độ dài tối thiểu của SecretKey (HMAC-SHA256 cần >= 256 bit)
        /// </summary>
        public const int MIN_SECRET_KEY_LENGTH = 32;

        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public int AccessTokenMinutes { get; set; } = 15;
        public int RefreshTokenDays { get; set; } = 7;
    }
}
