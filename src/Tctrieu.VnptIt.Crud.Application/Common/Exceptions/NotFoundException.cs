using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tctrieu.VnptIt.Crud.Application.Common.Exceptions
{
    /// <summary>
    /// Không tìm thấy dữ liệu (map sang HTTP 404)
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
