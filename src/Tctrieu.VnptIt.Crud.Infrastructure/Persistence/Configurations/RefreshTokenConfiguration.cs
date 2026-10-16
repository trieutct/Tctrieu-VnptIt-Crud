using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Infrastructure.Persistence.Configurations
{
    // Cấu hình map entity RefreshToken xuống bảng refresh_tokens
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");

            // Khóa chính tự tăng
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            // SHA256 dạng base64 dài 44 ký tự
            builder.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(64);

            // Tra cứu theo hash khi refresh / logout
            builder.HasIndex(x => x.TokenHash)
                .IsUnique();

            builder.Property(x => x.ReplacedByTokenHash)
                .HasMaxLength(64);

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.RevokedAt);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            // Các property tính toán, không lưu DB
            builder.Ignore(x => x.IsExpired);
            builder.Ignore(x => x.IsRevoked);
            builder.Ignore(x => x.IsActive);

            // Xóa user thì xóa luôn refresh token
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
