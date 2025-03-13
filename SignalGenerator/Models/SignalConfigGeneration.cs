namespace SignalGenerator.Models
{
    public class SignalConfigGeneration
    {
        public SignalConfigGeneration()
        {
            NumberOfSignals = 1;
            SignalCount = 1;
            MinFrequency = 58.5;
            MaxFrequency = 60.5;
            StepFrequency = 0.1;  // مقدار افزایش فرکانس بین هر سیگنال
            IntervalMs = 1000;  // فاصله زمانی بین ارسال سیگنال‌ها (میلی‌ثانیه)
            Signals = new List<double>();
            TimeRemainingSecs = 0;
            DistributionType = "Random";  // روش توزیع: "Random" یا "Sequential"
            UseFixedStep = true;  // اگر true باشد، از مقدار StepFrequency برای تولید سیگنال استفاده می‌شود
        }

        public int SignalCount { get; set; }  // تعداد سیگنال‌هایی که باید تولید شوند
        public double MinFrequency { get; set; }  // حداقل مقدار فرکانس
        public double MaxFrequency { get; set; }  // حداکثر مقدار فرکانس
        public double StepFrequency { get; set; }  // گام تغییر مقدار فرکانس
        public bool UseFixedStep { get; set; }  // آیا از گام ثابت برای تولید مقادیر استفاده شود؟
        public int IntervalMs { get; set; }  // فاصله زمانی بین هر ارسال (میلی‌ثانیه)
        public List<double> Signals { get; set; }  // لیست سیگنال‌های تولید شده
        public double TimeRemainingSecs { get; set; }  // زمان باقی‌مانده برای ارسال بعدی (ثانیه)
        public int NumberOfSignals { get; set; }  // تعداد کل سیگنال‌هایی که باید ارسال شوند
        public string DistributionType { get; set; }  // نوع توزیع سیگنال‌ها ("Random" یا "Sequential")
    }
}
