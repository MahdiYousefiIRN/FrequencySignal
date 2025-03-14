using EasyModbus;
using Microsoft.Extensions.Configuration;
using System;

namespace SignalGenerator.Modbus
{
    public class ModbusClientManager
    {
        private readonly ModbusClient modbusClient;
        private readonly string ip;
        private readonly int port;

        public bool IsConnected => modbusClient?.Connected ?? false;

        public ModbusClientManager(IConfiguration configuration)
        {
            // دریافت IP و Port از appsettings.json
            ip = configuration["Modbus:Ip"] ?? "127.0.0.1";  // مقدار پیش‌فرض
            port = int.TryParse(configuration["Modbus:Port"], out int parsedPort) ? parsedPort : 502; // مقدار پیش‌فرض

            modbusClient = new ModbusClient(ip, port);
        }

        // متد اتصال به سرور Modbus
        public void Connect()
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    modbusClient.Connect();
                    Console.WriteLine($"✅ Connected to Modbus Server at {ip}:{port}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Modbus Connection Error: {ex.Message}");
            }
        }

        // متد قطع اتصال
        public void Disconnect()
        {
            try
            {
                if (modbusClient.Connected)
                {
                    modbusClient.Disconnect();
                    Console.WriteLine("✅ Disconnected from Modbus Server.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Modbus Disconnection Error: {ex.Message}");
            }
        }

        // متد خواندن داده‌ها از Modbus
        public int[] ReadModbusData(int startAddress, int quantity)
        {
            try
            {
                if (modbusClient.Connected)
                {
                    return modbusClient.ReadHoldingRegisters(startAddress, quantity);
                }
                else
                {
                    Console.WriteLine("❌ Modbus Client is not connected!");
                    return new int[0]; // مقدار پیش‌فرض
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Modbus Read Error: {ex.Message}");
                return new int[0]; // مقدار پیش‌فرض
            }
        }

        // متد نوشتن داده‌ها در Modbus
        public void WriteModbusData(int startAddress, int[] data)
        {
            try
            {
                if (modbusClient.Connected)
                {
                    modbusClient.WriteMultipleRegisters(startAddress, data);
                    Console.WriteLine($"✅ Data written to Modbus at address {startAddress}");
                }
                else
                {
                    Console.WriteLine("❌ Modbus Client is not connected!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Modbus Write Error: {ex.Message}");
            }
        }

        // متد اتصال مجدد در صورت قطع ارتباط
        public void Reconnect()
        {
            Disconnect();
            Connect();
        }
    }
}
