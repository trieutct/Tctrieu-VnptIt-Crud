using System;

namespace Tctrieu.VnptIt.Crud.Application.Common.Exceptions
{
    /// <summary>
    /// Chưa xác thực hoặc thông tin xác thực không hợp lệ (map sang HTTP 401)
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
