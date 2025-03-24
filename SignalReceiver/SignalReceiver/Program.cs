using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using SignalReceiver.Modbus;
using System.Net.Http;
using SignalReceiver;

var builder = WebApplication.CreateBuilder(args);

// بارگذاری تنظیمات از فایل appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// افزودن سرویس‌های مورد نیاز به DI Container
builder.Services.AddRazorPages(); // فعال‌سازی Razor Pages (در صورت نیاز)
builder.Services.AddServerSideBlazor(); // فعال‌سازی Blazor Server
builder.Services.AddSignalR(); // افزودن SignalR برای ارتباطات بلادرنگ
builder.Services.AddHttpClient(); // پیکربندی HttpClient برای ارسال درخواست‌های HTTP

// پیکربندی و ثبت سرویس ModbusClientManager به‌صورت Singleton
builder.Services.AddSingleton(sp =>
    new ModbusClientManager(
        builder.Configuration["AppSettings:Modbus:Host"],
        int.Parse(builder.Configuration["AppSettings:Modbus:Port"])
    ));

// ثبت SignalReceiverService به‌صورت Scoped (هر درخواست یک نمونه جدید)
builder.Services.AddScoped<SignalReceiverService>();

var app = builder.Build();

// تنظیمات مربوط به محیط اجرایی (Production/Development)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); // هدایت به صفحه‌ی خطا در محیط Production
    app.UseHsts(); // فعال‌سازی HSTS برای امنیت بیشتر
}

// فعال‌سازی Middlewareهای ضروری
app.UseHttpsRedirection(); // تغییر درخواست‌های HTTP به HTTPS
app.UseStaticFiles(); // ارائه‌ی فایل‌های استاتیک (CSS, JS, تصاویر و...)
app.UseRouting(); // فعال‌سازی مسیریابی

// تنظیم مسیرهای مربوط به MVC، Razor Pages، Blazor و SignalR
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages(); // فعال‌سازی Razor Pages
app.MapBlazorHub(); // تنظیم Hub برای Blazor
app.MapHub<SignalHub>("/SignalHub"); // تنظیم مسیر Hub برای SignalR

// اجرای برنامه
app.Run();
