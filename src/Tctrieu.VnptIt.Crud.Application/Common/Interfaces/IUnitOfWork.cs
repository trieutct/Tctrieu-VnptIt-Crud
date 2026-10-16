using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tctrieu.VnptIt.Crud.Application.Common.Interfaces
{
    // Lưu toàn bộ thay đổi xuống database trong một lần
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
