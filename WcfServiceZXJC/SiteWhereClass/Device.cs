using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace WcfServiceZXJC
{
    public class Device
    {
        //public ObjectId _id { get; set; }
        //public string parentHardwareId { get; set; }
        //public string assignmentToken { get; set; }
        //public Dictionary<string,string> deviceElementMappings { get; set; }
        //public BsonDateTime createdDate { get; set; }
        //public BsonDateTime updatedDate { get; set; }





        public string createdDate { get; set; }
        public string updatedBy { get; set; }
        public string createdBy { get; set; }
        public bool deleted { get; set; }
        public string assetImageUrl { get; set; }
        public string hardwareId { get; set; }
        public string siteToken { get; set; }
        public string specificationToken { get; set; }
        public string comments { get; set; }
        public Dictionary<string,string> metadata { get; set; }
        public Assignments assignment { get; set; }
        public Specifications specification { get; set; }
    }
}
