using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Files;
using Tctrieu.VnptIt.Crud.Domain.Entities;
using Tctrieu.VnptIt.Crud.Infrastructure.Persistence.Mongo;

namespace Tctrieu.VnptIt.Crud.Infrastructure.Repositories
{
    // Lưu thông tin file vào collection "files" trong MongoDB
    public class FileRepository : IFileRepository
    {
        private readonly MongoDbContext _context;

        public FileRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<StoredFile?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            // Id không đúng định dạng ObjectId thì coi như không tìm thấy, tránh lỗi 500 khi serialize
            if (!ObjectId.TryParse(id, out _))
                return null;

            return await _context.Files.Find(x => x.Id == id).FirstOrDefaultAsync(ct);
        }

        public Task AddAsync(StoredFile file, CancellationToken ct = default)
            => _context.Files.InsertOneAsync(file, cancellationToken: ct);

        public Task DeleteAsync(string id, CancellationToken ct = default)
            => _context.Files.DeleteOneAsync(x => x.Id == id, ct);
    }
}
