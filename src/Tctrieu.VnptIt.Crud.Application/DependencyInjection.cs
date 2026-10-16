using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Auth;
using Tctrieu.VnptIt.Crud.Application.Users;

namespace Tctrieu.VnptIt.Crud.Application
{
    // Đăng ký service của tầng Application
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
