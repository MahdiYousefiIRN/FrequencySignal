namespace SignalGenerator.Models
{
    public class SignalConfigGeneration
    {
        public SignalConfigGeneration()
        {
            SignalCount = 1;  // تعداد سیگنال‌ها
            MinFrequency = 58.5;  // حداقل فرکانس
            MaxFrequency = 60.5;  // حداکثر فرکانس
            StepFrequency = 0.1;  // گام فرکانس
            IntervalMs = 1000;  // فاصله زمانی بین تولید سیگنال‌ها (میلی‌ثانیه)
            Signals = new List<double>();
            TimeRemainingSecs = 0;
            DistributionType = "Random";  // نوع توزیع: تصادفی یا ترتیبی
            UseFixedStep = true;  // آیا از گام ثابت استفاده شود؟
        }

        // تعداد سیگنال‌هایی که باید تولید شوند
        public int SignalCount { get; set; }

        // حداقل مقدار فرکانس
        public double MinFrequency { get; set; }

        // حداکثر مقدار فرکانس
        public double MaxFrequency { get; set; }

        // گام تغییر فرکانس بین هر سیگنال
        public double StepFrequency { get; set; }

        // آیا از گام ثابت برای تولید مقادیر استفاده شود؟
        public bool UseFixedStep { get; set; }

        // فاصله زمانی بین ارسال سیگنال‌ها (میلی‌ثانیه)
        public int IntervalMs { get; set; }

        // لیست سیگنال‌های تولید شده
        public List<double> Signals { get; set; }

        // زمان باقی‌مانده برای ارسال بعدی (ثانیه)
        public double TimeRemainingSecs { get; set; }

        // تعداد کل سیگنال‌هایی که باید ارسال شوند
        public int NumberOfSignals { get; set; }

        // نوع توزیع سیگنال‌ها ("Random" یا "Sequential")
        public string DistributionType { get; set; }
    }
}
