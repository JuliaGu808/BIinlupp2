using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionAppInlupp2.Models;
using System.Data.SqlClient;
using System;
using System.Globalization;

namespace FunctionAppInlupp2
{
    public static class SaveToSql2
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("SaveToSql2")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "iothub-ehub-iothubdemo-8676784-bb8643f378", ConsumerGroup = "testfunc")]EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
            var msg = JsonConvert.DeserializeObject<Messages>(Encoding.UTF8.GetString(message.Body.Array));
            using (var conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldbdemo2")))
            {
                var _dhtsql = "DECLARE @roleId int " +
                    "DECLARE @personId int " +
                    "DECLARE @deviceTypeId int " +
                    "DECLARE @geoLocationId int " +
                    "IF NOT EXISTS (SELECT Id FROM Roles2 WHERE RoleType = @roleType) " +
                    "INSERT INTO Roles2 VALUES(@roleType) " +
                    "SELECT @roleId = Id FROM Roles2 WHERE RoleType = @roleType " +
                    "IF NOT EXISTS (SELECT PersonId FROM Persons2 WHERE Name = @name) " +
                    "INSERT INTO Persons2 VALUES(@roleId, @name) " +
                    "SELECT @personId = PersonId FROM Persons2 WHERE Name = @name " +
                    "IF @roleType='student' IF NOT EXISTS(SELECT PersonId FROM Students2 WHERE PersonId = @personId) INSERT INTO Students2 VALUES(@personId, @school) " +
                    "IF @roleType='teacher' IF NOT EXISTS(SELECT PersonId FROM Teachers2 WHERE PersonId = @personId) INSERT INTO Teachers2 VALUES(@personId, @office) " +
                    "IF NOT EXISTS (SELECT Id FROM DeviceTypes2 WHERE Type = @deviceType) " +
                    "INSERT INTO DeviceTypes2 VALUES(@deviceType) " +
                    "SELECT @deviceTypeId = Id FROM DeviceTypes2 WHERE Type = @deviceType " +
                    "IF NOT EXISTS (SELECT Id FROM GeoLocations2 WHERE Latitude = @latitude AND Longitude = @longitude) " +
                    "INSERT INTO GeoLocations2 VALUES(@location, @latitude, @longitude) " +
                    "SELECT @geoLocationId = Id FROM GeoLocations2 WHERE Latitude = @latitude AND Longitude = @longitude " +
                    "IF NOT EXISTS (SELECT Id FROM Devices2 WHERE Id = @deviceId) " +
                    "INSERT INTO Devices2 VALUES(@deviceId, @deviceTypeId, @geoLocationId, @personId) " +
                    "INSERT INTO DhtMeasurements2 VALUES (@deviceId, @measurementTime, @temperature, @humidity)";

               
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(msg.ts);
                msg.utc = dateTimeOffset.UtcDateTime;
                string role = message.Properties["role"].ToString();
                conn.Open();


                using (var cmd = new SqlCommand(_dhtsql, conn))
                {


                    string school = "";
                    string office = "";
                    if (role == "student") school = message.Properties["school"].ToString();
                    if (role == "teacher") office = message.Properties["office"].ToString();
                    cmd.Parameters.AddWithValue("@school", school);
                    cmd.Parameters.AddWithValue("@office", office);
                    cmd.Parameters.AddWithValue("@temperature", msg.temperature);
                    cmd.Parameters.AddWithValue("@humidity", msg.humidity);
                    cmd.Parameters.AddWithValue("@measurementTime", msg.utc);
                    cmd.Parameters.AddWithValue("@deviceId", message.SystemProperties["iothub-connection-device-id"].ToString());
                    cmd.Parameters.AddWithValue("@latitude", float.Parse(message.Properties["latitude"].ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@longitude", float.Parse(message.Properties["longitude"].ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@deviceType", message.Properties["sensorType"].ToString());
                    cmd.Parameters.AddWithValue("@location", message.Properties["location"].ToString());
                    cmd.Parameters.AddWithValue("@name", message.Properties["name"].ToString());
                    cmd.Parameters.AddWithValue("@roleType", role);
                    cmd.ExecuteNonQuery();
                    log.LogInformation($"save to {role}");
                }
            }
        }
    }
}