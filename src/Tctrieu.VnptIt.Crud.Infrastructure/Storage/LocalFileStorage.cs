using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Files;

namespace Tctrieu.VnptIt.Crud.Infrastructure.Storage
{
    // Lưu file trên ổ đĩa server theo dạng {RootPath}/{guid}{ext}
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _rootPath;

        public LocalFileStorage(FileStorageSettings settings)
        {
            _rootPath = settings.GetFullRootPath();
        }

        public async Task<string> SaveAsync(Stream content, string extension, CancellationToken ct = default)
        {
            // Tên file sinh ngẫu nhiên, không dùng tên client gửi lên để tránh trùng và path traversal
            var stored_path = $"{Guid.NewGuid():N}{extension}";
            var full_path = Path.Combine(_rootPath, stored_path);

            Directory.CreateDirectory(_rootPath);

            await using var target = new FileStream(full_path, FileMode.CreateNew, FileAccess.Write);
            await content.CopyToAsync(target, ct);

            return stored_path;
        }

        public Stream? OpenRead(string storedPath)
        {
            var full_path = GetSafeFullPath(storedPath);
            if (!File.Exists(full_path))
                return null;

            return new FileStream(full_path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        }

        public void Delete(string storedPath)
        {
            var full_path = GetSafeFullPath(storedPath);
            if (File.Exists(full_path))
                File.Delete(full_path);
        }

        // Chặn đường dẫn trỏ ra ngoài thư mục gốc
        private string GetSafeFullPath(string storedPath)
        {
            var full_path = Path.GetFullPath(Path.Combine(_rootPath, storedPath));
            if (!full_path.StartsWith(_rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Đường dẫn file không hợp lệ");

            return full_path;
        }
    }
}
