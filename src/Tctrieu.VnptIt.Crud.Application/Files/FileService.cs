using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Common.Exceptions;
using Tctrieu.VnptIt.Crud.Application.Files.Dtos;
using Tctrieu.VnptIt.Crud.Domain.Entities;
using Tctrieu.VnptIt.Crud.Domain.Exceptions;

namespace Tctrieu.VnptIt.Crud.Application.Files
{
    // Xử lý nghiệp vụ upload/download file
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;
        private readonly FileStorageSettings _settings;

        public FileService(
            IFileRepository fileRepository,
            IFileStorage fileStorage,
            FileStorageSettings settings)
        {
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
            _settings = settings;
        }

        // Lưu file xuống ổ đĩa rồi ghi thông tin vào MongoDB
        public async Task<FileDto> UploadAsync(
            Stream content,
            string fileName,
            string contentType,
            long size,
            int userId,
            CancellationToken ct = default)
        {
            if (size <= 0)
                throw new DomainException("File rỗng");

            if (size > _settings.MaxFileSizeBytes)
                throw new DomainException($"File vượt quá dung lượng cho phép ({_settings.MaxFileSizeMb} MB)");

            // Chỉ lấy tên file, bỏ phần đường dẫn client gửi lên
            var original_name = Path.GetFileName(fileName ?? string.Empty);
            var extension = Path.GetExtension(original_name).ToLowerInvariant();
            if (!_settings.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                throw new DomainException($"Chỉ cho phép các định dạng: {string.Join(", ", _settings.AllowedExtensions)}");

            var stored_path = await _fileStorage.SaveAsync(content, extension, ct);

            try
            {
                var file = StoredFile.Create(original_name, stored_path, contentType, size, userId);
                await _fileRepository.AddAsync(file, ct);
                return ToDto(file);
            }
            catch
            {
                // Ghi MongoDB lỗi thì xóa file vừa lưu để không còn file mồ côi
                _fileStorage.Delete(stored_path);
                throw;
            }
        }

        // Lấy thông tin file
        public async Task<FileDto> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var file = await GetFileOrThrowAsync(id, ct);
            return ToDto(file);
        }

        // Mở nội dung file để trả về cho client
        public async Task<(Stream Content, FileDto File)> DownloadAsync(string id, CancellationToken ct = default)
        {
            var file = await GetFileOrThrowAsync(id, ct);

            var content = _fileStorage.OpenRead(file.StoredPath);
            if (content == null)
                throw new NotFoundException("File không còn tồn tại trên server");

            return (content, ToDto(file));
        }

        // Xóa file, chỉ người upload mới được xóa
        public async Task DeleteAsync(string id, int userId, CancellationToken ct = default)
        {
            var file = await GetFileOrThrowAsync(id, ct);

            if (file.UploadedBy != userId)
                throw new ForbiddenException("Bạn không có quyền xóa file này");

            await _fileRepository.DeleteAsync(file.Id, ct);
            _fileStorage.Delete(file.StoredPath);
        }

        private FileDto ToDto(StoredFile file)
            => FileDto.FromEntity(file, _settings.GetFileUrl(file.StoredPath));

        // Lấy file theo id, không có thì ném NotFoundException
        private async Task<StoredFile> GetFileOrThrowAsync(string id, CancellationToken ct)
        {
            var file = await _fileRepository.GetByIdAsync(id, ct);
            if (file == null)
                throw new NotFoundException($"Không tìm thấy file có id = {id}");

            return file;
        }
    }
}
