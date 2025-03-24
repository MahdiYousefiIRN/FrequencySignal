using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;

namespace SignalMonitor.Models
{
    /// <summary>
    /// مدل داده‌ای برای نمایش اطلاعات بسته‌های دریافتی از پروتکل‌های مختلف
    /// </summary>
    public class SignalData : INotifyPropertyChanged
    {
        private double _value; // مقدار عددی بسته
        private bool _isMalicious; // نشان‌دهنده این‌که بسته مشکوک یا مخرب است یا نه
        private string _status; // وضعیت بسته (مثلاً کامل، ناقص، معتبر، مخرب)
        private double _previousValue; // مقدار قبلی برای تشخیص تغییرات ناگهانی

        /// <summary>
        /// شناسه‌ی یکتا برای هر بسته
        /// </summary>
        public Guid PacketId { get; set; } = Guid.NewGuid();  // این شناسه به عنوان کلید اصلی استفاده می‌شود

        /// <summary>
        /// آدرس IP ارسال‌کننده بسته
        /// </summary>
        public string SourceIP { get; set; }

        /// <summary>
        /// آدرس IP دریافت‌کننده بسته (سرور)
        /// </summary>
        public string DestinationIP { get; set; }

        /// <summary>
        /// پروتکل ارتباطی که بسته از طریق آن ارسال شده است (مثل Modbus TCP، IEC 104، HTTP و ...)
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// مقدار عددی ارسال‌شده در بسته
        /// </summary>
        public double Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _previousValue = _value; // مقدار قبلی را ذخیره می‌کنیم
                    _value = value; // مقدار جدید را تنظیم می‌کنیم
                    OnPropertyChanged(nameof(Value)); // اطلاع‌رسانی برای به‌روزرسانی UI
                }
            }
        }

        /// <summary>
        /// مقدار قبلی بسته برای تشخیص تغییرات ناگهانی در داده‌ها
        /// </summary>
        public double PreviousValue
        {
            get => _previousValue;
            set
            {
                if (_previousValue != value)
                {
                    _previousValue = value;
                    OnPropertyChanged(nameof(PreviousValue)); // اطلاع‌رسانی برای تغییر مقدار
                }
            }
        }

        /// <summary>
        /// وضعیت بسته (کامل، ناقص، مخرب، معتبر)
        /// </summary>
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status)); // اطلاع‌رسانی برای تغییر وضعیت
                }
            }
        }

        /// <summary>
        /// آیا بسته مشکوک یا حمله‌ی هکری است؟
        /// </summary>
        public bool IsMalicious
        {
            get => _isMalicious;
            set
            {
                if (_isMalicious != value)
                {
                    _isMalicious = value;
                    OnPropertyChanged(nameof(IsMalicious)); // اطلاع‌رسانی برای تغییر وضعیت حمله
                }
            }
        }

        /// <summary>
        /// کلید امنیتی برای اعتبارسنجی بسته
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// زمان دریافت بسته در سرور (زمان دقیق دریافت بسته برای نمایش در UI)
        /// </summary>
        public DateTime ReceivedTimestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// رویداد تغییر مقدار برای بروزرسانی UI در Blazor
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// فراخوانی رویداد تغییر مقدار
        /// </summary>
        /// <param name="propertyName">نام ویژگی که تغییر کرده است</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

   
}
