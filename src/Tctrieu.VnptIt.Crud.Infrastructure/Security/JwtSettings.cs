namespace Tctrieu.VnptIt.Crud.Infrastructure.Security
{
    // Cấu hình JWT, đọc từ section "Jwt" trong appsettings.json
    public class JwtSettings
    {
        public const string SECTION_NAME = "Jwt";

        // Độ dài tối thiểu của SecretKey (HMAC-SHA256 cần >= 256 bit)
        public const int MIN_SECRET_KEY_LENGTH = 32;

        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public int AccessTokenMinutes { get; set; } = 15;
        public int RefreshTokenDays { get; set; } = 7;
    }
}
