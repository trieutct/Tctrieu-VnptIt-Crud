using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Tctrieu.VnptIt.Crud.Application.Files
{
    // Đọc/ghi nội dung file trên ổ đĩa server
    public interface IFileStorage
    {
        // Lưu stream xuống ổ đĩa, trả về đường dẫn tương đối so với thư mục gốc
        Task<string> SaveAsync(Stream content, string extension, CancellationToken ct = default);

        // Mở file để đọc, không tồn tại thì trả về null
        Stream? OpenRead(string storedPath);

        void Delete(string storedPath);
    }
}
