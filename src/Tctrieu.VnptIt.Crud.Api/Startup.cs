using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Api.Common.ModelBinding;
using Tctrieu.VnptIt.Crud.Api.Common.Responses;
using Tctrieu.VnptIt.Crud.Api.Middlewares;
using Tctrieu.VnptIt.Crud.Application;
using Tctrieu.VnptIt.Crud.Infrastructure;

namespace Tctrieu.VnptIt.Crud.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services
                .AddControllers(options =>
                {
                    // Query string snake_case: ?page_size=10
                    options.ValueProviderFactories.Add(new SnakeCaseQueryValueProviderFactory());
                })
                .AddJsonOptions(options =>
                {
                    // Enum trả về dạng chữ "Active" thay vì số
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // Lỗi validate trả về format { code, message, errors[] } thay vì ProblemDetails mặc định
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(x => x.Value.Errors.Count > 0)
                            .SelectMany(x => x.Value.Errors.Select(e => new ErrorDetail
                            {
                                // .NET 5 trả key theo tên property C# (FullName), đổi sang snake_case cho khớp JSON
                                Key = ToSnakeCase(x.Key),
                                ErrorCode = StatusCodes.Status400BadRequest,
                                Message = e.ErrorMessage,
                            }))
                            .ToList();

                        return new BadRequestObjectResult(
                            ApiResponse.Error(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ", errors));
                    };
                });
            services.AddApplication();
            services.AddInfrastructure(Configuration);
            AddJwtAuthentication(services);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tctrieu.VnptIt.Crud.Api", Version = "v1" });

                // Nút Authorize trên Swagger để gắn Bearer token
                var bearer_scheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Nhập access token (không cần tiền tố \"Bearer \")",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme,
                    },
                };
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, bearer_scheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [bearer_scheme] = new List<string>(),
                });
            });
        }

        // Cấu hình xác thực JWT, lỗi 401/403 trả về format { code, message, errors }
        private void AddJwtAuthentication(IServiceCollection services)
        {
            var jwt_settings = Infrastructure.DependencyInjection.GetJwtSettings(Configuration);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt_settings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwt_settings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_settings.SecretKey)),
                        ValidateLifetime = true,
                        // Mặc định cho lệch 5 phút, bỏ để token hết hạn đúng giờ
                        ClockSkew = TimeSpan.Zero,
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            // Chặn response mặc định (body rỗng)
                            context.HandleResponse();

                            var message = context.AuthenticateFailure is SecurityTokenExpiredException
                                ? "Access token đã hết hạn"
                                : "Chưa đăng nhập hoặc token không hợp lệ";

                            await WriteErrorAsync(context.Response, StatusCodes.Status401Unauthorized, message);
                        },
                        OnForbidden = context =>
                            WriteErrorAsync(context.Response, StatusCodes.Status403Forbidden, "Không có quyền truy cập"),
                    };
                });

            services.AddAuthorization();
        }

        // FullName => full_name, PageSize => page_size
        private static string ToSnakeCase(string value)
            => Regex.Replace(value, "(?<=[a-z0-9])([A-Z])", "_$1").ToLowerInvariant();

        private static Task WriteErrorAsync(HttpResponse response, int statusCode, string message)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            return response.WriteAsync(JsonSerializer.Serialize(ApiResponse.Error(statusCode, message)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Đặt đầu tiên để bắt được lỗi của mọi middleware phía sau
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tctrieu.VnptIt.Crud.Api v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
