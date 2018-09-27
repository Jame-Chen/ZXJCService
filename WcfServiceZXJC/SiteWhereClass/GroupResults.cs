using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceZXJC
{
    public class GroupResults
    {
        public string numResults { get; set; }
        public List<Group> results { get; set; }
    }

    public class Group
    {
        public string type { get; set; }
        //public int index { get; set; }
        public string token { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<string> roles { get; set; }
        public string groupToken { get; set; }
        public string elementId { get; set; }
    }
    public class Group2Results
    {
        public string numResults { get; set; }
        public List<Group2> results { get; set; }
    }
    public class Group2
    {
        public string type { get; set; }
        public string token { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<string> roles { get; set; }
        public string groupToken { get; set; }
        public string elementId { get; set; }

        public Dictionary<string,string> metadata { get; set; }
    }
}
