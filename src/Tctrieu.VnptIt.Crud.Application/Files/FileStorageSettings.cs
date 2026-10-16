using System.IO;

namespace Tctrieu.VnptIt.Crud.Application.Files
{
    // Cấu hình lưu file, đọc từ section "FileStorage" trong appsettings.json
    public class FileStorageSettings
    {
        public const string SECTION_NAME = "FileStorage";

        // Thư mục lưu file, đường dẫn tương đối tính từ thư mục chạy ứng dụng
        public string RootPath { get; set; } = "uploads";
        // Đường dẫn public để xem file: {RequestPath}/{stored_path}
        public string RequestPath { get; set; } = "/uploads";
        public int MaxFileSizeMb { get; set; } = 10;
        // Để rỗng vì binder .NET 5 nối thêm phần tử từ config vào mảng có sẵn, rỗng thì dùng DEFAULT_ALLOWED_EXTENSIONS
        public string[] AllowedExtensions { get; set; } = System.Array.Empty<string>();

        public static readonly string[] DEFAULT_ALLOWED_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".xlsx" };

        public long MaxFileSizeBytes => MaxFileSizeMb * 1024L * 1024L;

        // Đường dẫn tuyệt đối của thư mục lưu file
        public string GetFullRootPath() => Path.GetFullPath(RootPath, Directory.GetCurrentDirectory());

        // URL tương đối để xem file, ví dụ /uploads/abc.png
        public string GetFileUrl(string storedPath) => $"{RequestPath.TrimEnd('/')}/{storedPath}";
    }
}
