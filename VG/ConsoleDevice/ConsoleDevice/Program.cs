using ConsoleDevice.model;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDevice
{  
    class Program
    {
        private static DeviceClient deviceClient;
        

        static async Task Main(string[] args)
        {
            string conn = getConn();
            deviceClient = DeviceClient.CreateFromConnectionString(conn, TransportType.Mqtt);

            while (true)
            {
                await SendMessageAsync(getJson());
                await Task.Delay(12000);
            }
            
        }

        private static string getConn()
        {
            string connect = "";
            Connstr conn = new Connstr();
            var ss = conn.GetType();
            PropertyInfo[] props = ss.GetProperties();
            foreach (var prop in props)
            {
                if (prop.GetIndexParameters().Length == 0)
                {
                    connect = prop.GetValue(conn).ToString();
                    //Console.WriteLine("   {0} ({1}): {2}", prop.Name,
                    //                  prop.PropertyType.Name,
                    //                  prop.GetValue(conn));

                }
            }
            return connect;
        }

        private static string getJson()
        {
            Random random = new Random();
            int max = 8;
            int min = 5;
            double temp = (random.NextDouble() * (max - min) + min);
            max = 40;
            min = 20;
            double hum = (random.NextDouble() * (max - min) + min);

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;

            var message = new
            {
                temperature = (float)temp,
                humidity = (float)hum,
                ts = secondsSinceEpoch
            };

            return JsonConvert.SerializeObject(message);
        }

        private static async Task SendMessageAsync(string message)
        {
            var msg = new Message(Encoding.UTF8.GetBytes(message));
            msg.Properties.Add("sensorType", "dht");
            msg.Properties.Add("name", "Hans");
            msg.Properties.Add("office", "Nackademin");
            msg.Properties.Add("role", "teacher");
            msg.Properties.Add("location", "malmö");
            msg.Properties.Add("latitude", "55.5791");
            msg.Properties.Add("longitude", "13.0109");                                                                                                                                         
            await deviceClient.SendEventAsync(msg);
        }
    }
}
