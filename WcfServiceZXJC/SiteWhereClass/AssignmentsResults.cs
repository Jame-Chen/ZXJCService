using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceZXJC
{
    public class AssignmentsResults
    {
        public string numResults { get; set; }
        public List<Assignments> results { get; set; }
    }

    public class Assignments
    {
        public string token { get; set; }
        public string deviceHardwareId { get; set; }
        public string assignmentType { get; set; }
        public string assetModuleId { get; set; }
        public string assetId { get; set; }
        public Dictionary<string, string> metadata { get; set; }
        public State state { get; set; }
    }
}
