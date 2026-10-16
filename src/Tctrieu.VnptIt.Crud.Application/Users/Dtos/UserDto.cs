using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Domain.Entities;
using Tctrieu.VnptIt.Crud.Domain.Enums;

namespace Tctrieu.VnptIt.Crud.Application.Users.Dtos
{
    // Thông tin user trả về cho client
    public class UserDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = default!;
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;
        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }
        [JsonPropertyName("status")]
        public UserStatusEnum Status { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // Chuyển từ entity sang DTO
        public static UserDto FromEntity(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }
    }
}
