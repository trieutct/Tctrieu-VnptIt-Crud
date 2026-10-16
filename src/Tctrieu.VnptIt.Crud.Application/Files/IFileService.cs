using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Files.Dtos;

namespace Tctrieu.VnptIt.Crud.Application.Files
{
    // Nghiệp vụ upload/download file
    public interface IFileService
    {
        Task<FileDto> UploadAsync(Stream content, string fileName, string contentType, long size, int userId, CancellationToken ct = default);
        Task<FileDto> GetByIdAsync(string id, CancellationToken ct = default);
        Task<(Stream Content, FileDto File)> DownloadAsync(string id, CancellationToken ct = default);
        Task DeleteAsync(string id, int userId, CancellationToken ct = default);
    }
}
