using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Domain.Common;
using Tctrieu.VnptIt.Crud.Domain.Enums;
using Tctrieu.VnptIt.Crud.Domain.Exceptions;

namespace Tctrieu.VnptIt.Crud.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public string? PhoneNumber { get; private set; }
        public UserStatusEnum Status { get; private set; }

        // Constructor rỗng cho EF Core
        private User()
        {
        }

        // Tạo user mới, kiểm tra dữ liệu đầu vào
        public static User Create(
            string fullName,
            string email,
            string passwordHash,
            string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Họ tên không được để trống");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email không được để trống");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Mật khẩu không hợp lệ");

            return new User
            {
                FullName = fullName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                PhoneNumber = phoneNumber?.Trim(),
                Status = UserStatusEnum.Active,
            };
        }

        // Cập nhật thông tin cơ bản
        public void UpdateProfile(string fullName, string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Họ tên không được để trống");

            FullName = fullName.Trim();
            PhoneNumber = phoneNumber?.Trim();
            MarkUpdated();
        }

        // Khóa tài khoản
        public void Deactivate()
        {
            if (Status == UserStatusEnum.Inactive) return;

            Status = UserStatusEnum.Inactive;
            MarkUpdated();
        }

        // Mở khóa tài khoản
        public void Activate()
        {
            if (Status == UserStatusEnum.Active) return;

            Status = UserStatusEnum.Active;
            MarkUpdated();
        }
    }

}
