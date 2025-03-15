namespace SignalGenerator.Models
{
    public class SignalMonitor
    {
        public SignalMonitor()
        {
            Frequency = 0.0;  // فرکانس سیگنال
            Power = 0.0;  // توان سیگنال
            Timestamp = DateTime.UtcNow;  // زمان ارسال سیگنال
            Status = "Inactive";  // وضعیت سیگنال (فعال یا غیرفعال)
            SignalId = 0;  // شناسه سیگنال
            IsSentSuccessfully = false;  // وضعیت ارسال سیگنال
            EventType = "Generated";  // نوع رویداد (برای لاگ)
            EventMessage = string.Empty;  // پیام لاگ
            LogTime = DateTime.UtcNow;  // زمان لاگ‌گیری
        }

        // فرکانس سیگنال (برای مانیتورینگ)
        public double Frequency { get; set; }

        // توان سیگنال (اختیاری)
        public double Power { get; set; }

        // زمان ارسال سیگنال
        public DateTime Timestamp { get; set; }

        // وضعیت سیگنال (فعال یا غیرفعال)
        public string Status { get; set; }

        // شناسه سیگنال (اختیاری برای تمایز سیگنال‌ها)
        public int SignalId { get; set; }

        // آیا سیگنال موفق به ارسال شده یا خیر
        public bool IsSentSuccessfully { get; set; }

        // نوع رویداد (برای لاگ‌گیری)
        public string EventType { get; set; }  // مثلاً "Generated", "Sent", "Failed"

        // پیام لاگ برای ثبت رویداد
        public string EventMessage { get; set; }

        // زمان لاگ‌گیری (برای ثبت تاریخ و ساعت رویداد)
        public DateTime LogTime { get; set; }

        // فرکانس و توان برای رسم نمودار چارت
        public string ChartLabel => $"{Timestamp:HH:mm:ss} - Freq: {Frequency} Hz, Power: {Power} W";
    }
}
