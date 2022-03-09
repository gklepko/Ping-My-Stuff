using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;

namespace Ping_My_Stuff
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PrintAppInfo();

            try
            {
                var lines = System.IO.File.ReadAllLines("devices.txt");
                var devices = new List<Device>();

                foreach (var line in lines)
                {               
                    if (line.StartsWith("#") || line.Length == 0)
                    {
                        continue;
                    }

                    var deviceRawInfo = line.Split();
                    devices.Add(new Device(deviceRawInfo[0], deviceRawInfo[1]));
                }

                foreach (var device in devices)
                {
                    Console.WriteLine(device);
                    device.Ping();
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        private static void PrintAppInfo()
        { 
            var name = Assembly.GetExecutingAssembly().GetName().Name.ToString();
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{name} v{version}");
            Console.ResetColor();
        }
    }

    class Device
    {
        public string Name { get; }
        public string IPAddress { get; }
        public Device(String name, String ipAddress)
        {
            Name = name;
            IPAddress = ipAddress;
        }
        public async void Ping()
        {
            var ping = new Ping();

            var timeout = TimeSpan.FromSeconds(5);

            var reply = await ping.SendPingAsync(IPAddress, (int)timeout.TotalSeconds);

            if (reply.Status == IPStatus.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Name: {Name} Status: {reply.Status} RoundTrip Time: {reply.RoundtripTime}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Name: {Name} Status: {reply.Status} RoundTrip Time: {reply.RoundtripTime}");
            }

            Console.ResetColor();
        }
        public override string ToString()
        {
            return $"{Name} {IPAddress}";
        }
    }
}
