using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceZXJC
{
    public class DeviceMeasurements
    {
        public string id { get; set; }
        public string alternateId { get; set; }
        public string eventType { get; set; }
        public string siteToken { get; set; }
        public string deviceAssignmentToken { get; set; }
        public string assignmentType { get; set; }
        public string assetModuleId { get; set; }
        public string assetId { get; set; }
        public string eventDate { get; set; }
        public string receivedDate { get; set; }
        public Dictionary<string, double> measurements { get; set; }
        public string measurementsSummary { get; set; }
        public bool updateState { get; set; }
        public Dictionary<string, double> metadata { get; set; }
    }

   
}
