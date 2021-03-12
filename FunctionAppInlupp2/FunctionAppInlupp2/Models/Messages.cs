using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionAppInlupp2.Models
{
    public class Messages
    {      
        public long ts { get; set; }
        public float humidity { get; set; }
        public float temperature { get; set; }
        public DateTime utc { get; set; }
    }
}
