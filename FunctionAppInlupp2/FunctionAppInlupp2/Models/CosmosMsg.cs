using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionAppInlupp2.Models
{
    public class CosmosMsg
    {
        public string deviceId { get; set; }
        public string location { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public DateTime utc { get; set; }
    }
}
