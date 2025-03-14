using Microsoft.AspNetCore.SignalR;
using SignalGenerator.Modbus;
using SignalGenerator.Services;

var builder = WebApplication.CreateBuilder(args);

// بارگذاری تنظیمات از فایل appsettings.json
// این خط برای خواندن تنظیمات از فایل پیکربندی است. اگر فایل وجود نداشته باشد، خطا نمی‌دهد و اگر تغییر کند، به‌روزرسانی می‌شود.
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// افزودن سرویس HttpClient به DI برای استفاده در سرویس‌های مختلف
// این سرویس برای ارتباط با APIهای خارجی و تعامل با سرورها مورد استفاده قرار می‌گیرد.
builder.Services.AddHttpClient(); // ✅ برای ارسال درخواست‌های HTTP به سایر سرویس‌ها و سرورها

// افزودن سرویس‌های Razor و Blazor
// برای استفاده از صفحات Razor در برنامه
builder.Services.AddRazorPages(); // برای افزودن قابلیت‌های Razor Pages به اپلیکیشن

// برای استفاده از Blazor Server
// این سرویس برای ایجاد اپلیکیشن‌های Blazor Server به‌کار می‌رود که در آن تعاملات UI به صورت real-time از طریق SignalR مدیریت می‌شود.
builder.Services.AddServerSideBlazor(); // برای راه‌اندازی Blazor Server

// افزودن SignalR برای برقراری ارتباطات real-time
// SignalR به شما امکان می‌دهد داده‌ها را به‌صورت real-time به کلاینت‌ها ارسال کنید.
builder.Services.AddSignalR(); // ✅ برای پشتیبانی از ارتباطات real-time بین سرور و کلاینت‌ها

// افزودن Singleton برای مدیریت Modbus
// این سرویس برای مدیریت ارتباطات Modbus در برنامه است که باید به‌صورت یک نمونه Singleton در دسترس باشد.
builder.Services.AddSingleton<ModbusClientManager>(); // ✅ به‌صورت Singleton برای مدیریت یک ارتباط پایدار Modbus

// افزودن Singleton برای سرویس تولید سیگنال
// این سرویس وظیفه تولید سیگنال‌ها و ارسال آنها به SignalR و Modbus را بر عهده دارد.
builder.Services.AddSingleton<SignalGeneratorService>(); // ✅ به‌صورت Singleton برای ایجاد و ارسال سیگنال‌ها به سیستم‌های دیگر

// ساخت اپلیکیشن
var app = builder.Build();

// پیکربندی درخواست‌های HTTP برای محیط تولید
if (!app.Environment.IsDevelopment())
{
    // مدیریت خطاها در محیط‌های غیر توسعه‌ای
    // در صورت بروز خطا در سرور، کاربر به صفحه "Error" هدایت می‌شود.
    app.UseExceptionHandler("/Error"); // نمایش صفحه خطا در صورت بروز استثنا

    // فعال‌سازی HSTS (HTTP Strict Transport Security) برای افزایش امنیت
    // این پروتکل از حملات "Man-in-the-Middle" جلوگیری می‌کند.
    app.UseHsts(); // اجباری کردن استفاده از HTTPS

}

// هدایت تمام درخواست‌ها به HTTPS
// به‌محض دریافت درخواست HTTP، سرور درخواست را به HTTPS هدایت می‌کند.
app.UseHttpsRedirection(); // هدایت درخواست‌ها به HTTPS

// سرو فایل‌های استاتیک (CSS، JavaScript، تصاویر و غیره)
app.UseStaticFiles(); // برای ارائه فایل‌های استاتیک از دایرکتوری wwwroot

// فعال‌سازی مسیریابی برای درخواست‌ها
// این سرویس، درخواست‌ها را به مسیرهای مناسب هدایت می‌کند.
app.UseRouting(); // فعال‌سازی مسیریابی برای درخواست‌های HTTP

// مپ کردن Blazor Hub برای Blazor Server
// این خط ارتباطات Blazor را تنظیم می‌کند و اجازه می‌دهد که UI به صورت real-time به روزرسانی شود.
app.MapBlazorHub(); // اتصال Blazor Hub برای ارتباطات real-time

// مپ کردن SignalR Hub برای ارسال و دریافت داده‌ها
// SignalR Hub برای ارتباطات real-time بین سرور و کلاینت‌ها استفاده می‌شود.
app.MapHub<SignalHub>("/signalHub"); // مسیر SignalR Hub برای ارتباطات real-time

// برای مسیر پیش‌فرض (Fallback)، صفحه _Host را نمایش می‌دهد
// این مسیر در صورتی که مسیر دیگری پیدا نشود، صفحه اصلی Blazor را نمایش می‌دهد.
app.MapFallbackToPage("/_Host"); // نمایش صفحه _Host در صورت عدم یافتن مسیر

// اجرای اپلیکیشن
app.Run(); // اجرای اپلیکیشن و شروع پردازش درخواست‌ها
