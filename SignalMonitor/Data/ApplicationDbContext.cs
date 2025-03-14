using Microsoft.EntityFrameworkCore;
using SignalMonitor.Models;
using System.Collections.Generic;

namespace SignalMonitor.Data
{
    // در اینجا باید ApplicationDbContext را تنظیم کنید
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PacketData> PacketData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تنظیم کلید اصلی برای PacketData
            modelBuilder.Entity<PacketData>()
                .HasKey(p => p.PacketId);  // تنظیم PacketId به عنوان کلید اصلی
        }
    }
}
