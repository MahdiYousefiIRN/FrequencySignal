using EasyModbus;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SignalReceiver.Modbus
{
    public class ModbusClientManager
    {
        private ModbusClient _modbusClient;
        private readonly string _host;
        private readonly int _port;

        public ModbusClientManager(string host, int port)
        {
            _host = host;
            _port = port;
        }

        // اتصال به Modbus
        public void Connect()
        {
            _modbusClient = new ModbusClient(_host, _port);
            _modbusClient.Connect();
        }

        // خواندن داده‌ها از Modbus
        public List<double> ReadSignalData(int startAddress, int numOfPoints)
        {
            if (_modbusClient != null && _modbusClient.Connected)
            {
                var data = _modbusClient.ReadHoldingRegisters(startAddress, numOfPoints);
                return data.Select(d => (double)d).ToList(); // تبدیل داده‌ها به نوع double
            }
            throw new Exception("Modbus client is not connected.");
        }

        // قطع اتصال از Modbus
        public void Disconnect()
        {
            _modbusClient?.Disconnect();
        }
    }
}
