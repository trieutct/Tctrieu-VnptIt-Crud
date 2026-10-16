using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Domain.Entities;
using Tctrieu.VnptIt.Crud.Domain.Enums;

namespace Tctrieu.VnptIt.Crud.Infrastructure.Persistence.Configurations
{
    // Cấu hình map entity User xuống bảng users
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            // Khóa chính tự tăng
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            // Email không được trùng
            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20);

            // Lưu enum dạng chuỗi in hoa: ACTIVE / INACTIVE
            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion(
                    v => v.ToString().ToUpperInvariant(),
                    v => Enum.Parse<UserStatusEnum>(v, true));

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);
        }
    }
}
