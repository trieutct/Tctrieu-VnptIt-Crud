using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Api.Common.Authorization;
using Tctrieu.VnptIt.Crud.Api.Common.Extensions;
using Tctrieu.VnptIt.Crud.Api.Common.Responses;
using Tctrieu.VnptIt.Crud.Application.Files;
using Tctrieu.VnptIt.Crud.Application.Files.Dtos;

namespace Tctrieu.VnptIt.Crud.Api.Controllers
{
    // API upload/download file, file lưu trên server, thông tin file lưu trong MongoDB
    [ApiController]
    [Authorize(Policy = Policies.ACTIVE_USER)]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // Upload 1 file (multipart/form-data, field "file")
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResponse<FileDto>>> Upload(
            [FromForm] UploadFileRequest request,
            CancellationToken ct)
        {
            await using var stream = request.File.OpenReadStream();
            var file = await _fileService.UploadAsync(
                stream,
                request.File.FileName,
                request.File.ContentType,
                request.File.Length,
                User.GetUserId(),
                ct);
            file.Url = ToAbsoluteUrl(file.Url);

            return CreatedAtAction(
                nameof(GetById),
                new { id = file.Id },
                ApiResponse.Success(file, "Upload file thành công", StatusCodes.Status201Created));
        }

        // Lấy thông tin file
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<FileDto>>> GetById(string id, CancellationToken ct)
        {
            var file = await _fileService.GetByIdAsync(id, ct);
            file.Url = ToAbsoluteUrl(file.Url);
            return Ok(ApiResponse.Success(file));
        }

        // Tải nội dung file
        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(string id, CancellationToken ct)
        {
            var (content, file) = await _fileService.DownloadAsync(id, ct);

            // FileStreamResult tự dispose stream sau khi trả về
            return File(content, file.ContentType, file.FileName);
        }

        // Xóa file, chỉ người upload mới được xóa
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(string id, CancellationToken ct)
        {
            await _fileService.DeleteAsync(id, User.GetUserId(), ct);
            return Ok(ApiResponse.Success("Xóa file thành công"));
        }

        // /uploads/abc.png => https://host/uploads/abc.png
        private string ToAbsoluteUrl(string relativeUrl)
            => $"{Request.Scheme}://{Request.Host}{Request.PathBase}{relativeUrl}";
    }

    // Body upload file
    public class UploadFileRequest
    {
        [JsonPropertyName("file")]
        [Required(ErrorMessage = "Vui lòng chọn file")]
        public IFormFile File { get; set; } = default!;
    }
}
