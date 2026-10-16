using System;

namespace Tctrieu.VnptIt.Crud.Application.Common.Exceptions
{
    /// <summary>
    /// Đã xác thực nhưng không được phép, ví dụ tài khoản bị khóa (map sang HTTP 403)
    /// </summary>
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {
        }
    }
}
