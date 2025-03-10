using Microsoft.AspNetCore.SignalR;
using SignalGenerator.Service;

var builder = WebApplication.CreateBuilder(args);

// افزودن سرویس‌های مورد نیاز
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<SignalGeneratorService>(); // ✅ اضافه شد
builder.Services.AddSignalR();

var app = builder.Build();

// پیکربندی درخواست‌های HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// مپ کردن Blazor و SignalR
app.MapBlazorHub();
app.MapHub<SignalHub>("/signalHub");
app.MapFallbackToPage("/_Host");

app.Run();
