using System;

namespace Tctrieu.VnptIt.Crud.Application.Common.Exceptions
{
    // Đã xác thực nhưng không được phép, ví dụ tài khoản bị khóa (map sang HTTP 403)
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {
        }
    }
}
