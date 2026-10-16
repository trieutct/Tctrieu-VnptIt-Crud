using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Common.Interfaces;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Infrastructure.Persistence
{
    // DbContext làm việc với MySQL
    public class AppDbContext : DbContext, IUnitOfWork
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tự nạp toàn bộ file trong thư mục Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
