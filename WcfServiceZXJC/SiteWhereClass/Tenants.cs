using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfServiceZXJC
{
    public class Tenants
    {
        public string id { get; set; }
        public string name { get; set; }
        public string authenticationToken { get; set; }
        public string logoUrl { get; set; }
        public List<string> authorizedUserIds { get; set; }

        public string tenantTemplateId { get; set; }
    }
}