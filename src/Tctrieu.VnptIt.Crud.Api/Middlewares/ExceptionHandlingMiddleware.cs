using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Api.Common.Responses;
using Tctrieu.VnptIt.Crud.Application.Common.Exceptions;
using Tctrieu.VnptIt.Crud.Domain.Exceptions;

namespace Tctrieu.VnptIt.Crud.Api.Middlewares
{
    /// <summary>
    /// Bắt mọi exception và trả về JSON lỗi thống nhất
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Log thông tin request
                _logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);

                // Chỉ log, không chặn authentication
                await _next(context);

                // Log thông tin response
                _logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        // Map loại exception sang HTTP status
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var (status_code, message) = ex switch
            {
                DomainException => (StatusCodes.Status400BadRequest, ex.Message),
                NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                ConflictException => (StatusCodes.Status409Conflict, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Lỗi hệ thống, vui lòng thử lại sau"),
            };

            // Chỉ log chi tiết với lỗi không lường trước
            if (status_code == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Lỗi chưa xử lý");
            else
                _logger.LogWarning("{Message}", ex.Message);

            context.Response.StatusCode = status_code;
            context.Response.ContentType = "application/json";

            var body = JsonSerializer.Serialize(ApiResponse.Error(status_code, message));

            await context.Response.WriteAsync(body);
        }
    }
}
