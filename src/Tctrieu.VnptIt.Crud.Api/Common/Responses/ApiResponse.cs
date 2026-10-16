using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tctrieu.VnptIt.Crud.Api.Common.Responses
{
    /// <summary>
    /// Shape response thành công chuẩn: { code, message, data }
    /// </summary>
    public class ApiResponse<T>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }

        public ApiResponse(T data, string message = ApiResponse.DefaultSuccessMessage, int code = StatusCodes.Status200OK)
        {
            Code = code;
            Message = message;
            Data = data;
        }
    }

    /// <summary>
    /// Dữ liệu dạng danh sách kèm tổng số bản ghi: { items, total_items, ...additionalInfo }
    /// </summary>
    public class CommonListResponse<T>
    {
        [JsonPropertyName("items")]
        public IEnumerable<T> Items { get; set; }

        [JsonPropertyName("total_items")]
        public int TotalItems { get; set; }

        // Các field phụ (page, page_size, ...) được trải phẳng vào cùng cấp với items
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalInfo { get; set; }
    }

    /// <summary>
    /// Chi tiết 1 lỗi cụ thể (ví dụ lỗi từng field khi validate)
    /// </summary>
    public class ErrorDetail
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object Data { get; set; }
    }

    /// <summary>
    /// Shape response lỗi chuẩn: { code, message, errors }
    /// </summary>
    public class ErrorResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("errors")]
        public IEnumerable<ErrorDetail> Errors { get; set; }

        public ErrorResponse(
            int code = StatusCodes.Status500InternalServerError,
            string message = "",
            IEnumerable<ErrorDetail> errors = null)
        {
            Code = code;
            Message = message;
            Errors = errors ?? new List<ErrorDetail>();
        }
    }

    /// <summary>
    /// Factory tạo response, dùng trong controller
    /// </summary>
    public static class ApiResponse
    {
        public const string DefaultSuccessMessage = "success";

        /// <summary>
        /// Bọc response thành công: { code, message, data }
        /// </summary>
        public static ApiResponse<T> Success<T>(T data, string message = DefaultSuccessMessage, int code = StatusCodes.Status200OK)
            => new ApiResponse<T>(data, message, code);

        /// <summary>
        /// Response thành công không có dữ liệu (data = {})
        /// </summary>
        public static ApiResponse<object> Success(string message = DefaultSuccessMessage)
            => new ApiResponse<object>(new { }, message);

        /// <summary>
        /// Bọc response danh sách: { code, message, data: { items, total_items, ...additionalInfo } }
        /// </summary>
        public static ApiResponse<CommonListResponse<T>> SuccessList<T>(
            IEnumerable<T> items,
            int total,
            Dictionary<string, object> additionalInfo = null,
            string message = DefaultSuccessMessage)
        {
            var data = new CommonListResponse<T>
            {
                Items = items,
                TotalItems = total,
                AdditionalInfo = additionalInfo,
            };
            return new ApiResponse<CommonListResponse<T>>(data, message);
        }

        /// <summary>
        /// Bọc response lỗi: { code, message, errors }
        /// </summary>
        public static ErrorResponse Error(
            int code = StatusCodes.Status500InternalServerError,
            string message = "",
            IEnumerable<ErrorDetail> errors = null)
            => new ErrorResponse(code, message, errors);
    }
}
