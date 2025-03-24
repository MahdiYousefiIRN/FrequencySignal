using Microsoft.EntityFrameworkCore;
using SignalGenerator.Modbus;
using SignalGenerator.signalLogs;
using Microsoft.Extensions.Configuration;
using SignalGenerator.Models;
using SignalGenerator;

var builder = WebApplication.CreateBuilder(args);

// بارگذاری تنظیمات از فایل appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// افزودن خدمات مختلف به DI container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();

// پیکربندی Modbus
builder.Services.AddSingleton<ModbusClientManager>(); // ثبت Singleton برای ModbusClientManager
builder.Services.Configure<ModbusOptions>(builder.Configuration.GetSection("Modbus")); // پیکربندی تنظیمات Modbus

// افزودن SignalLog DbContext و Repository
builder.Services.AddDbContext<SignalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // اتصال به دیتابیس

builder.Services.AddScoped<ISignalRepository, SignalRepository>(); // ثبت ISignalRepository و پیاده‌سازی آن
builder.Services.AddScoped<SignalGeneratorService>(); // ثبت SignalGeneratorService

// پیکربندی API برای ارسال داده‌ها
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AppSettings:ApiUrl"]);
});

// ساخت اپلیکیشن
var app = builder.Build();

// پیکربندی درخواست‌ها در محیط‌های مختلف
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// مپ کردن Blazor Hub و SignalR Hub
app.MapBlazorHub();
app.MapHub<SignalHub>("/signalHub");

// مسیر صفحه پیش‌فرض
app.MapFallbackToPage("/_Host");

// اجرای اپلیکیشن
app.Run();
