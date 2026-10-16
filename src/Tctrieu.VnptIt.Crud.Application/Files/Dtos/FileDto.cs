using System;
using System.Text.Json.Serialization;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Application.Files.Dtos
{
    // Thông tin file trả về cho client
    public class FileDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;
        [JsonPropertyName("file_name")]
        public string FileName { get; set; } = default!;
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; } = default!;
        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;
        [JsonPropertyName("size")]
        public long Size { get; set; }
        [JsonPropertyName("uploaded_by")]
        public int UploadedBy { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        // Chuyển từ entity sang DTO
        public static FileDto FromEntity(StoredFile file, string url)
        {
            return new FileDto
            {
                Id = file.Id,
                FileName = file.OriginalName,
                Url = url,
                ContentType = file.ContentType,
                Size = file.Size,
                UploadedBy = file.UploadedBy,
                CreatedAt = file.CreatedAt,
            };
        }
    }
}
