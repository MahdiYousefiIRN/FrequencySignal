﻿using Microsoft.AspNetCore.SignalR.Client;

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SignalReceiver.Models;
using Microsoft.AspNetCore.Mvc;

public class SignalReceiverService
{
    private HubConnection _hubConnection;
    private HttpClient _httpClient;
    private List<double> _signalData;
    private string _modbusUrl = "http://localhost:5002/modbus";  // URL فرضی برای Modbus

    // افزودن IConfiguration به سازنده سرویس برای دسترسی به تنظیمات
    public SignalReceiverService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _signalData = new List<double>();

        // دریافت URL SignalR از appsettings.json
        var signalRHubUrl = configuration["AppSettings:SignalRHubUrl"];

        // ایجاد و تنظیم اتصال SignalR به URL مناسب
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(signalRHubUrl)  // استفاده از آدرس دریافت شده از appsettings.json
            .Build();

        // تعریف متدهایی که باید هنگام دریافت داده از SignalR اجرا شوند
        _hubConnection.On<List<double>>("ReceiveSignalData", (signalData) =>
        {
            // پردازش داده‌های دریافتی از SignalR
            Console.WriteLine("Signal data received from SignalR: " + string.Join(",", signalData));

            // ذخیره داده‌ها در متغیر _signalData
            _signalData = signalData;
        });
    }

    // متد برای شروع دریافت سیگنال‌ها از SignalR
    public async Task StartReceivingSignalFromSignalRAsync()
    {
        try
        {
            // تلاش برای شروع اتصال به SignalR
            await _hubConnection.StartAsync();
            Console.WriteLine("SignalR connection started.");
        }
        catch (Exception ex)
        {
            // در صورت بروز خطا هنگام اتصال به SignalR
            Console.WriteLine($"Error starting SignalR connection: {ex.Message}");
        }
    }

    // متد برای قطع اتصال از SignalR
    public async Task StopReceivingSignalFromSignalRAsync()
    {
        if (_hubConnection != null)
        {
            try
            {
                // قطع اتصال از SignalR
                await _hubConnection.StopAsync();
                // آزادسازی منابع مرتبط با اتصال
                await _hubConnection.DisposeAsync();
                Console.WriteLine("SignalR connection stopped.");
            }
            catch (Exception ex)
            {
                // در صورت بروز خطا هنگام قطع اتصال
                Console.WriteLine($"Error stopping SignalR connection: {ex.Message}");
            }
        }
    }

    // متد برای دریافت سیگنال‌ها از API
    public async Task<List<double>> GetSignalFromApi([FromBody] SignalRequestDto signalRequestDto)
    {
        try
        {
            if (signalRequestDto == null || signalRequestDto.SignalData == null || !signalRequestDto.SignalData.Any())
            {
                Console.WriteLine("Error: No valid signal data received from API.");
                return new List<double>();
            }

            foreach (var item in signalRequestDto.SignalData)
            {
                Console.WriteLine($"Data: {item}, Timestamp: {signalRequestDto.Timestamp}");
            }

            return signalRequestDto.SignalData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving signal from API: {ex.Message}");
            return new List<double>();
        }
    }


    // متد برای دریافت سیگنال‌ها از Modbus
    public async Task<List<double>> ReceiveSignalFromModbus()
    {
        try
        {
            // ارسال درخواست به سرور Modbus و دریافت داده‌ها
            var response = await _httpClient.GetStringAsync(_modbusUrl);

            // پردازش داده‌های دریافتی از Modbus
            var signals = JsonSerializer.Deserialize<List<double>>(response);

            return signals ?? new List<double>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving signal from Modbus: {ex.Message}");
            return new List<double>();
        }
    }

    // متد برای دریافت داده‌های سیگنال
    public List<double> GetSignalData()
    {
        return _signalData;
    }

}
