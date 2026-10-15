using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Common.Interfaces;

namespace Tctrieu.VnptIt.Crud.Infrastructure.Security
{
    /// <summary>
    /// Mã hóa mật khẩu bằng PBKDF2-SHA256
    /// Định dạng lưu: {iterations}.{salt_base64}.{hash_base64}
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Độ dài salt (byte)
        /// </summary>
        private const int SALT_SIZE = 16;

        /// <summary>
        /// Độ dài hash (byte)
        /// </summary>
        private const int HASH_SIZE = 32;

        /// <summary>
        /// Số vòng lặp, càng cao càng khó dò nhưng càng chậm
        /// </summary>
        private const int ITERATIONS = 100_000;

        public string Hash(string password)
        {
            // Sinh salt ngẫu nhiên cho mỗi mật khẩu
            var salt = new byte[SALT_SIZE];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hash = HashWithSalt(password, salt, ITERATIONS);

            return $"{ITERATIONS}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool Verify(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash)) return false;

            var parts = passwordHash.Split('.');
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out var iterations)) return false;

            try
            {
                var salt = Convert.FromBase64String(parts[1]);
                var expected_hash = Convert.FromBase64String(parts[2]);
                var actual_hash = HashWithSalt(password, salt, iterations);

                // So sánh thời gian cố định, tránh tấn công dò theo thời gian
                return CryptographicOperations.FixedTimeEquals(actual_hash, expected_hash);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // Băm mật khẩu với salt cho trước
        private static byte[] HashWithSalt(string password, byte[] salt, int iterations)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(HASH_SIZE);
        }
    }
}
