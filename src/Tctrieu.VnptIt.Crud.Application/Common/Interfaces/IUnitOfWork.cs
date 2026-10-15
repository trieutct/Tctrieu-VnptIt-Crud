using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tctrieu.VnptIt.Crud.Application.Common.Interfaces
{
    /// <summary>
    /// Lưu toàn bộ thay đổi xuống database trong một lần
    /// </summary>
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
