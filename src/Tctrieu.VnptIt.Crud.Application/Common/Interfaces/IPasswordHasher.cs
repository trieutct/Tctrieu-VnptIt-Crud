using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tctrieu.VnptIt.Crud.Application.Common.Interfaces
{
    // Mã hóa và kiểm tra mật khẩu
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
