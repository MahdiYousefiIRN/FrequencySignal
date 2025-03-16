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

// افزودن خدمات مختلف به DI container
builder.Services.AddRazorPages(); // فعال سازی Razor Pages (در صورتی که نیاز دارید)
builder.Services.AddServerSideBlazor(); // فعال سازی Blazor Server Side (در صورتی که نیاز دارید)
builder.Services.AddSignalR(); // اضافه کردن SignalR

// پیکربندی Modbus
builder.Services.AddSingleton<ModbusClientManager>(sp =>
    new ModbusClientManager(builder.Configuration["AppSettings:Modbus:Host"],
                           int.Parse(builder.Configuration["AppSettings:Modbus:Port"])));
builder.Services.AddScoped<SignalReceiverService>(); // اضافه کردن سرویس SignalReceiver
builder.Services.AddHttpClient(); // پیکربندی HTTP Client

// ساخت اپلیکیشن
var app = builder.Build();

// پیکربندی درخواست‌ها در محیط‌های مختلف
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // استفاده از فایل‌های استاتیک
app.UseRouting(); // فعال سازی مسیریابی

// تنظیم روت‌ها و مسیرهای مختلف

// پیکربندی مسیر برای MVC (در صورت استفاده از MVC Controller)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// پیکربندی مسیر برای Razor Pages (اگر نیاز دارید)
app.MapRazorPages(); // مسیریابی برای Razor Pages

// مپ کردن Blazor Hub و SignalR Hub
app.MapBlazorHub(); // اضافه کردن Hub برای Blazor (در صورت نیاز)
app.MapHub<SignalHub>("/signalHub"); // اضافه کردن Hub برای SignalR

// حذف این خط در صورت استفاده از Web API بدون Blazor
// app.MapFallbackToPage("/_Host"); 

// اجرای اپلیکیشن
app.Run();
