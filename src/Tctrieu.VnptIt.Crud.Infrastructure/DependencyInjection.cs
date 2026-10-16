using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Auth;
using Tctrieu.VnptIt.Crud.Application.Common.Interfaces;
using Tctrieu.VnptIt.Crud.Application.Files;
using Tctrieu.VnptIt.Crud.Application.Users;
using Tctrieu.VnptIt.Crud.Infrastructure.Persistence;
using Tctrieu.VnptIt.Crud.Infrastructure.Persistence.Mongo;
using Tctrieu.VnptIt.Crud.Infrastructure.Repositories;
using Tctrieu.VnptIt.Crud.Infrastructure.Security;
using Tctrieu.VnptIt.Crud.Infrastructure.Storage;

namespace Tctrieu.VnptIt.Crud.Infrastructure
{
    // Đăng ký service của tầng Infrastructure
    public static class DependencyInjection
    {
        // Phiên bản MySQL đang dùng (khớp với image mysql trong docker-compose)
        private static readonly Version MYSQL_VERSION = new Version(8, 0, 36);

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connection_string = configuration.GetConnectionString("MySql");
            if (string.IsNullOrWhiteSpace(connection_string))
                throw new InvalidOperationException("Chưa cấu hình ConnectionStrings:MySql trong appsettings.json");

            // Kết nối MySQL qua Pomelo
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connection_string, new MySqlServerVersion(MYSQL_VERSION)));

            // Dùng chung một instance AppDbContext cho UnitOfWork trong cùng request
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            // Cấu hình JWT dùng chung cho TokenService và JwtBearer ở tầng Api
            services.AddSingleton(GetJwtSettings(configuration));
            services.AddSingleton<ITokenService, TokenService>();

            // MongoDB lưu thông tin file, nội dung file lưu trên ổ đĩa server
            services.AddSingleton(GetMongoSettings(configuration));
            services.AddSingleton<MongoDbContext>();
            services.AddScoped<IFileRepository, FileRepository>();

            var file_storage_settings = configuration.GetSection(FileStorageSettings.SECTION_NAME).Get<FileStorageSettings>()
                ?? new FileStorageSettings();
            if (file_storage_settings.AllowedExtensions.Length == 0)
                file_storage_settings.AllowedExtensions = FileStorageSettings.DEFAULT_ALLOWED_EXTENSIONS;
            services.AddSingleton(file_storage_settings);
            services.AddSingleton<IFileStorage, LocalFileStorage>();

            return services;
        }

        // Đọc và kiểm tra cấu hình MongoDB, thiếu thì dừng ứng dụng ngay khi khởi động
        private static MongoSettings GetMongoSettings(IConfiguration configuration)
        {
            var settings = configuration.GetSection(MongoSettings.SECTION_NAME).Get<MongoSettings>();

            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("Chưa cấu hình MongoDb:ConnectionString trong appsettings.json");

            if (string.IsNullOrWhiteSpace(settings.DatabaseName))
                throw new InvalidOperationException("Chưa cấu hình MongoDb:DatabaseName trong appsettings.json");

            return settings;
        }

        // Đọc và kiểm tra cấu hình JWT, thiếu thì dừng ứng dụng ngay khi khởi động
        public static JwtSettings GetJwtSettings(IConfiguration configuration)
        {
            var settings = configuration.GetSection(JwtSettings.SECTION_NAME).Get<JwtSettings>();

            if (settings == null || string.IsNullOrWhiteSpace(settings.SecretKey))
                throw new InvalidOperationException("Chưa cấu hình Jwt:SecretKey trong appsettings.json");

            if (settings.SecretKey.Length < JwtSettings.MIN_SECRET_KEY_LENGTH)
                throw new InvalidOperationException($"Jwt:SecretKey phải có tối thiểu {JwtSettings.MIN_SECRET_KEY_LENGTH} ký tự");

            return settings;
        }
    }
}
