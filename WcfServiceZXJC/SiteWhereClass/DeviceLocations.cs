using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceZXJC
{
   public class DeviceLocations
    {
        public string alternateId { get; set; }
        public string eventDate { get; set; }
        public bool updateState { get; set; }

        public Dictionary<string,string> metadata { get; set; }

        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public decimal elevation { get; set; }
    }
}
