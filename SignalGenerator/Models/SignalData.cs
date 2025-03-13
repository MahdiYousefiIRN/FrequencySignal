namespace SignalGenerator.Models
{
    public class SignalData
    {
        public int Id { get; set; }
        public double Frequency { get; set; }  // فرکانس سیگنال
        public DateTime Timestamp { get; set; }  // زمان ارسال سیگنال
        public string ServerAddress { get; set; }  // آدرس سرور
        public int PacketSize { get; set; }  // اندازه بسته
        public bool IsSentToServer { get; set; }  // وضعیت ارسال سیگنال به سرور
        public int SentPacketsCount { get; set; }  // تعداد بسته‌های ارسال شده
        public double TimeRemainingInSecs { get; set; }  // زمان باقی‌مانده برای ارسال سیگنال‌ها (ثانیه)
        public string StatusMessage { get; set; }  // پیام وضعیت ارسال سیگنال
    }
}
