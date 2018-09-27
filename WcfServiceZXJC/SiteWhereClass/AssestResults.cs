using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceZXJC
{
    public class AssestResults
    {
        public string numResults { get; set; }
        public List<Assest> results { get; set; }
    }


    public class Assest
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string assetCategoryId { get; set; }
        public string imageUrl { get; set; }
        public Dictionary<string, string> properties { get; set; }
        public string sku { get; set; }
        public string description { get; set; }
    }
}
