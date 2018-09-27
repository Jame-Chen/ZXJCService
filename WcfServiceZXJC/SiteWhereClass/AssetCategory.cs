using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceZXJC
{
    public class AssetCategoryResults {
        public int numResults { get; set; }

        public List<AssetCategory> results { get; set; }
    }
   public  class AssetCategory
    {
        public string id { get; set; }
        public string name { get; set; }
        public string assetType { get; set; }
    }
}
