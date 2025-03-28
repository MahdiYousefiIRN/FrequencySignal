﻿using Microsoft.EntityFrameworkCore;
using SignalMonitor.Data;
using SignalMonitor.SignalR;

namespace SignalMonitor
{
    public class Program
    {
        public static async Task Main(string[] args) // تغییر Main به async
        {
            var builder = WebApplication.CreateBuilder(args);

            // پیکربندی DbContext برای اتصال به پایگاه داده
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // اضافه کردن SignalR به DI container
            builder.Services.AddSignalR();

            // سایر تنظیمات
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // تنظیم مسیر برای کنترلر Home و SignalData
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            // اضافه کردن مسیر اختصاصی برای SignalData
            app.MapControllerRoute(
                name: "signalReceiver",
                pattern: "SignalReceiver/{action=Index}/{id?}",
                defaults: new { controller = "SignalReceiver" });

            // تنظیم مسیر برای Hub SignalR
            app.MapHub<SignalHub>("/SignalR/SignalHub");

            app.Run();
        }
    }
}
