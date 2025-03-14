using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SignalRClientService
{
    private HubConnection? _hubConnection;

    public async Task ConnectToSignalR()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/signalhub")  // آدرس سرور SignalR
            .WithAutomaticReconnect() // اضافه کردن قابلیت اتصال مجدد خودکار
            .Build();

        _hubConnection.On<List<double>>("ReceiveSignalData", (signalData) =>
        {
            // پردازش داده‌ها در اینجا
            Console.WriteLine($"Received signal data: {string.Join(", ", signalData)}");
            // ذخیره داده‌ها در دیتابیس یا پردازش‌های دیگر
        });

        _hubConnection.Closed += async (error) =>
        {
            Console.WriteLine($"Connection lost: {error?.Message}");
            await Task.Delay(5000); // ۵ ثانیه صبر کن، بعد دوباره وصل شو
            await StartConnectionAsync();
        };

        await StartConnectionAsync();
    }

    private async Task StartConnectionAsync()
    {
        try
        {
            await _hubConnection!.StartAsync();
            Console.WriteLine("SignalR connected.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to SignalR: {ex.Message}");
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("SendMessage", message);
            Console.WriteLine($"Message sent: {message}");
        }
        else
        {
            Console.WriteLine("Cannot send message, SignalR is not connected.");
        }
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            Console.WriteLine("SignalR disconnected.");
        }
    }
}
