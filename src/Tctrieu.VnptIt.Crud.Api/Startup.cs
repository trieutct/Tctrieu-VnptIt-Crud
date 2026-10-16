using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
                .AddControllers()
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
                                Key = x.Key,
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tctrieu.VnptIt.Crud.Api", Version = "v1" });
            });
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
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
