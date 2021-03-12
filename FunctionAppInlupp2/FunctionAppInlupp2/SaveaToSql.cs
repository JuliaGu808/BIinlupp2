using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System;
using Newtonsoft.Json;
using System.Globalization;
using FunctionAppInlupp2.Models;

namespace FunctionAppInlupp2
{
    public static class SaveaToSql
    {
        private static HttpClient client = new HttpClient();

         [FunctionName("SaveToSql")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "iothub-ehub-iothubdemo-8676784-bb8643f378", ConsumerGroup = "azurefunction")]EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
            //{"temperature":26.4,"humidity":26,"ts":1615152365}
            using (var conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldbdemo2")))
            {
                var _queryDeviceType = "IF NOT EXISTS (SELECT Id FROM DeviceTypes WHERE Type = @deviceType) INSERT INTO DeviceTypes OUTPUT inserted.id VALUES(@deviceType) ELSE SELECT * FROM DeviceTypes";
                var _queryGeoLocation = "IF NOT EXISTS (SELECT Id FROM GeoLocations WHERE Latitude = @latitude AND Longitude = @longitude) INSERT INTO GeoLocations OUTPUT inserted.Id VALUES(@latitude, @longitude) ELSE SELECT * FROM GeoLocations WHERE Latitude = @latitude AND Longitude = @longitude";
                var _queryDevice = "IF NOT EXISTS (SELECT Id FROM Devices WHERE Id = @deviceId) INSERT INTO Devices OUTPUT inserted.Id VALUES(@deviceId, @deviceTypeId, @geoLocationId) ELSE SELECT * FROM Devices WHERE Id = @deviceId";
                var _queryDhtMeasurement = "INSERT INTO DhtMeasurements VALUES (@deviceId, @measurementTime, @temperature, @humidity)";

                int _deviceTypeId;
                int _geoLocationId;
                string _deviceId;

                conn.Open();
                using (var cmd = new SqlCommand(_queryDeviceType, conn))
                {
                    cmd.Parameters.AddWithValue("@deviceType", message.Properties["sensorType"].ToString());
                    _deviceTypeId = int.Parse(cmd.ExecuteScalar().ToString());
                }
                using (var cmd = new SqlCommand(_queryGeoLocation, conn))
                {
                    cmd.Parameters.AddWithValue("@latitude", float.Parse(message.Properties["latitude"].ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@longitude", float.Parse(message.Properties["longitude"].ToString(), CultureInfo.InvariantCulture));
                    _geoLocationId = int.Parse(cmd.ExecuteScalar().ToString());
                }
                using (var cmd = new SqlCommand(_queryDevice, conn))
                {
                    cmd.Parameters.AddWithValue("@deviceId", message.SystemProperties["iothub-connection-device-id"].ToString());
                    cmd.Parameters.AddWithValue("@deviceTypeId", _deviceTypeId);
                    cmd.Parameters.AddWithValue("@geoLocationId", _geoLocationId);
                    _deviceId = cmd.ExecuteScalar().ToString();
                }
                using (var cmd = new SqlCommand(_queryDhtMeasurement, conn))
                {
                    var data = JsonConvert.DeserializeObject<Messages>(Encoding.UTF8.GetString(message.Body.Array));
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(data.ts);
                    data.utc = dateTimeOffset.DateTime;

                    cmd.Parameters.AddWithValue("@deviceId", _deviceId);
                    cmd.Parameters.AddWithValue("@measurementTime", data.utc);
                    cmd.Parameters.AddWithValue("@temperature", data.temperature);
                    cmd.Parameters.AddWithValue("@humidity", data.humidity);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}