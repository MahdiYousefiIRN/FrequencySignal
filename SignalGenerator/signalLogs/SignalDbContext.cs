using Microsoft.EntityFrameworkCore;

namespace SignalGenerator.signalLogs
{
    public class SignalDbContext : DbContext
    {
        public SignalDbContext(DbContextOptions<SignalDbContext> options) : base(options) { }

        public DbSet<SignalLog> SignalLogs { get; set; } // مشخص کردن مدل SignalLog برای ذخیره‌سازی در دیتابیس
    }
}
