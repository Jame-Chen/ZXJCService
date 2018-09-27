using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceZXJC
{
    public class ImpBatch
    {
        public string hardwareId { get; set; }
        public List<DeviceMeasurements> measurements { get; set; }
        public List<DeviceLocations> locations { get; set; }
    }
}
