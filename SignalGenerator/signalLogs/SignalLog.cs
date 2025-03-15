using System;

namespace SignalGenerator.signalLogs
{
    public class SignalLog
    {
        // شناسه منحصر به فرد برای هر لاگ سیگنال
        public int Id { get; set; }

        // زمان تولید سیگنال
        public DateTime Timestamp { get; set; }

        // فرکانس تولید شده
        public double Frequency { get; set; }

        // نوع سیگنال (مثلاً سیگنال تولید شده، خطا و غیره)
        public string SignalType { get; set; }

        // وضعیت ارسال به مقصد (موفق یا ناموفق)
        public string Status { get; set; }

        // پیام خطا در صورت بروز مشکل
        public string? ErrorMessage { get; set; }

        // سازنده پیش‌فرض
        public SignalLog()
        {
            Timestamp = DateTime.Now;
            SignalType = "Generated"; // فرض بر این است که سیگنال تولید شده باشد
            Status = "Success"; // پیش‌فرض وضعیت موفق
        }

        // سازنده با پارامترها
        public SignalLog(double frequency, string signalType, string status, string? errorMessage = null)
        {
            Timestamp = DateTime.Now;
            Frequency = frequency;
            SignalType = signalType;
            Status = status;
            ErrorMessage = errorMessage;
        }
    }
}
