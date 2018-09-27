using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceZXJC
{
  public  class Sites
     {
         public string token { get; set; }
         public string name { get; set; }
         public string description { get; set; }
         public string imageUrl { get; set; }
         public Map map { get; set; }
         public Dictionary<string,string> metadata { get; set; }
    }
    public class Map {
        public string type { get; set; }
        public Dictionary<string, string> metadata { get; set; }
    }
}
