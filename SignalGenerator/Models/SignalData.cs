namespace SignalGenerator.Models
{
    public class SignalData
    {
        public int Id { get; set; }  // شناسه سیگنال

        // اطلاعات مربوط به داده ارسالی
        public double Frequency { get; set; }  // مقدار فرکانس شبکه (هرتز)
        public double Voltage { get; set; }  // مقدار ولتاژ شبکه (ولت)
        public double Current { get; set; }  // مقدار جریان شبکه (آمپر)
        public double ActivePower { get; set; }  // توان اکتیو (کیلووات)
        public double ReactivePower { get; set; }  // توان راکتیو (کیلووار)
        public DateTime Timestamp { get; set; }  // زمان دریافت و ارسال داده

        // اطلاعات مربوط به ارسال داده
        public string ServerAddress { get; set; }  // آدرس IP یا دامنه سرور مقصد
        public int ServerPort { get; set; }  // شماره پورت مقصد
        public string Protocol { get; set; }  // نوع پروتکل ارتباطی (Modbus TCP, HTTP, WebSocket, MQTT, gRPC)
        public int PacketSize { get; set; }  // اندازه بسته‌های داده‌ای (بایت)
        public int SendIntervalMs { get; set; }  // فاصله زمانی ارسال داده (میلی‌ثانیه)
        public bool IsAsync { get; set; }  // آیا ارسال داده به‌صورت ناهمزمان (Async) است؟
        public bool RequiresAcknowledgment { get; set; }  // آیا سرور باید تأیید دریافت ارسال کند؟
        public int RetryCount { get; set; }  // تعداد دفعات تلاش مجدد در صورت شکست ارسال

        // اطلاعات امنیتی
        public bool RequiresAuthentication { get; set; }  // آیا ارسال داده نیاز به احراز هویت دارد؟
        public string AuthToken { get; set; }  // توکن احراز هویت برای ارسال داده (در صورت نیاز)
        public string EncryptionMethod { get; set; }  // نوع رمزگذاری داده (AES, RSA, TLS)

        // وضعیت ارسال
        public bool IsSentToServer { get; set; }  // آیا داده ارسال شده است؟
        public int SentPacketsCount { get; set; }  // تعداد بسته‌های ارسال شده موفق
        public double TimeRemainingInSecs { get; set; }  // زمان باقی‌مانده برای ارسال بعدی (ثانیه)
        public string StatusMessage { get; set; }  // پیام وضعیت ارسال

        // وضعیت سرور و شبکه
        public bool IsServerReachable { get; set; }  // آیا سرور مقصد در دسترس است؟
        public int ResponseTimeMs { get; set; }  // زمان پاسخ سرور (میلی‌ثانیه)
        public string LastErrorMessage { get; set; }  // آخرین خطای ارسال در صورت وجود
        public DateTime LastSuccessfulSend { get; set; }  // زمان آخرین ارسال موفق
    }
}
