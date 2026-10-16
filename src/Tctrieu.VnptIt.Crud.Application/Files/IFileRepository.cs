using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Application.Files
{
    // Truy cập thông tin file trong MongoDB
    public interface IFileRepository
    {
        Task<StoredFile?> GetByIdAsync(string id, CancellationToken ct = default);
        Task AddAsync(StoredFile file, CancellationToken ct = default);
        Task DeleteAsync(string id, CancellationToken ct = default);
    }
}
