using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Common.Interfaces;
using Tctrieu.VnptIt.Crud.Application.Users;
using Tctrieu.VnptIt.Crud.Infrastructure.Persistence;
using Tctrieu.VnptIt.Crud.Infrastructure.Repositories;
using Tctrieu.VnptIt.Crud.Infrastructure.Security;

namespace Tctrieu.VnptIt.Crud.Infrastructure
{
    /// <summary>
    /// Đăng ký service của tầng Infrastructure
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Phiên bản MySQL đang dùng (khớp với image mysql trong docker-compose)
        /// </summary>
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
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}
