using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SignalGenerator.signalLogs
{
    public class SignalRepository : ISignalRepository
    {
        private readonly SignalDbContext _context;

        // دریافت SignalDbContext از DI
        public SignalRepository(SignalDbContext context)
        {
            _context = context;
        }

        // متد ذخیره‌سازی سیگنال به‌صورت غیرهمزمان
        public async Task SaveSignalLogAsync(SignalLog signalLog)
        {
            // افزودن سیگنال به دیتابیس
            await _context.SignalLogs.AddAsync(signalLog);
            await _context.SaveChangesAsync(); // ذخیره تغییرات
        }
    }
}
