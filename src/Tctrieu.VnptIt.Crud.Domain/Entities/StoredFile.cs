using System;
using Tctrieu.VnptIt.Crud.Domain.Exceptions;

namespace Tctrieu.VnptIt.Crud.Domain.Entities
{
    // Thông tin file đã upload, lưu trong MongoDB (nội dung file nằm trên ổ đĩa server)
    public class StoredFile
    {
        public string Id { get; private set; } = default!;
        public string OriginalName { get; private set; } = default!;
        public string StoredPath { get; private set; } = default!;
        public string ContentType { get; private set; } = default!;
        public long Size { get; private set; }
        public int UploadedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Constructor rỗng cho MongoDB driver
        private StoredFile()
        {
        }

        // Tạo bản ghi file mới, Id do MongoDB sinh khi insert
        public static StoredFile Create(
            string originalName,
            string storedPath,
            string contentType,
            long size,
            int uploadedBy)
        {
            if (string.IsNullOrWhiteSpace(originalName))
                throw new DomainException("Tên file không được để trống");

            if (size <= 0)
                throw new DomainException("File rỗng");

            return new StoredFile
            {
                OriginalName = originalName.Trim(),
                StoredPath = storedPath,
                ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
                Size = size,
                UploadedBy = uploadedBy,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
