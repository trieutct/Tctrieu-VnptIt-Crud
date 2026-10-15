using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tctrieu.VnptIt.Crud.Application.Common.Exceptions
{
    /// <summary>
    /// Dữ liệu bị trùng, ví dụ email đã tồn tại (map sang HTTP 409)
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}
