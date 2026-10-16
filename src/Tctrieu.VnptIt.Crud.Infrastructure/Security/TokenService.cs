using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tctrieu.VnptIt.Crud.Application.Common.Interfaces;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Infrastructure.Security
{
    /// <summary>
    /// Sinh JWT access token (HMAC-SHA256) và refresh token ngẫu nhiên
    /// </summary>
    public class TokenService : ITokenService
    {
        /// <summary>
        /// Độ dài refresh token (byte)
        /// </summary>
        private const int REFRESH_TOKEN_SIZE = 64;

        private readonly JwtSettings _settings;
        private readonly SigningCredentials _signingCredentials;

        public TokenService(JwtSettings settings)
        {
            _settings = settings;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        public (string Token, DateTime ExpiresAt) CreateAccessToken(User user)
        {
            var now = DateTime.UtcNow;
            var expires_at = now.AddMinutes(_settings.AccessTokenMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: now,
                expires: expires_at,
                signingCredentials: _signingCredentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expires_at);
        }

        public (string Token, DateTime ExpiresAt) CreateRefreshToken()
        {
            var bytes = new byte[REFRESH_TOKEN_SIZE];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            // Base64Url để token dùng được trong URL/header mà không cần encode
            var token = Base64UrlEncoder.Encode(bytes);
            return (token, DateTime.UtcNow.AddDays(_settings.RefreshTokenDays));
        }

        public string HashRefreshToken(string refreshToken)
        {
            // Token đã đủ ngẫu nhiên nên SHA256 là đủ, không cần salt
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToBase64String(hash);
        }
    }
}
