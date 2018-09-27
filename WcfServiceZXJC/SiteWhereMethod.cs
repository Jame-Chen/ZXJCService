using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WcfServiceZXJC
{
    public class SiteWhereMethod
    {
        private string httpurl;
        private string tenanttoken;
        private string excludeAssignedPageSize;
        GetPost gp = new GetPost();
        public SiteWhereMethod()
        {
            httpurl = ConfigurationManager.AppSettings["httpurl"];
            tenanttoken = ConfigurationManager.AppSettings["tenanttoken"];
            excludeAssignedPageSize = ConfigurationManager.AppSettings["excludeAssignedPageSize"];
        }

        public void UpdateFlag(string table, string where)
        {
            string sql = "update " + table + " set S_UpdateFlag='0'  where " + where;
            DbHelperSQL.ExecuteSql(sql);
        }
        /// <summary>
        /// 返回token
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        public void UpdateToken(string table, string setwhere, string where)
        {
            string sql = "update " + table + " set " + setwhere + "  where " + where;
            DbHelperSQL.ExecuteSql(sql);
        }
        public string getTenantById(string Id)
        {

            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/tenants/" + Id, "", tenanttoken);
            return ret;
        }

        public string createTenant(string id, string name, string logoUrl, string authenticationToken, string authorizedUserIds)
        {
            Tenants tenants = new Tenants();
            tenants.id = id;
            tenants.name = name;
            tenants.logoUrl = logoUrl;
            tenants.authenticationToken = authenticationToken;
            List<string> userids = new List<string>();
            userids.Add(authorizedUserIds);
            tenants.authorizedUserIds = userids;
            tenants.tenantTemplateId = "empty";
            string data = JsonConvert.SerializeObject(tenants);

            data = gp.HttpPost("http://" + httpurl + "/sitewhere/api/tenants", data, tenanttoken);
            return data;
        }

        public string updateTenant(string id, string name, string logoUrl, string authenticationToken, string authorizedUserIds)
        {
            Tenants tenants = new Tenants();
            tenants.id = id;
            tenants.name = name;
            tenants.logoUrl = logoUrl;
            tenants.authenticationToken = authenticationToken;
            List<string> userids = new List<string>();

            userids.Add(authorizedUserIds);
            tenants.authorizedUserIds = userids;
            string data = JsonConvert.SerializeObject(tenants);

            data = gp.HttpPut("http://" + httpurl + "/sitewhere/api/tenants/" + id, data, tenanttoken);
            return data;
        }

        public string getSiteByToken(string siteToken)
        {
            siteToken = string.IsNullOrEmpty(siteToken) ? Guid.NewGuid().ToString("N") : siteToken;
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/sites/" + siteToken, "", tenanttoken);
            return ret;
        }

        public string createSite(string token, string imageUrl, string name, string description, Dictionary<string, string> dic)
        {
            Sites s = new Sites();
            s.token = token;
            s.imageUrl = imageUrl;
            s.name = name;
            s.description = description;
         
            s.metadata = dic;
            Map m = new Map();
            m.metadata = dic;
            s.map = m;
            string data = JsonConvert.SerializeObject(s);

            string ret = gp.HttpPost("http://" + httpurl + "/sitewhere/api/sites", data, tenanttoken);
            return ret;
        }

        public string updateSite(string token, string imageUrl, string name, string description,Dictionary<string, string> dic)
        {
            Sites s = new Sites();
            s.token = token;
            s.imageUrl = imageUrl;
            s.name = name;
            s.description = description;
            
            s.metadata = dic;
            Map m = new Map();
            m.metadata = dic;
            s.map = m;
            string data = JsonConvert.SerializeObject(s);

            string ret = gp.HttpPut("http://" + httpurl + "/sitewhere/api/sites/" + token, data, tenanttoken);
            return ret;
        }

        public string listAssetCategories()
        {
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/assets/categories", "", tenanttoken);
            return ret;
        }

        public string listCategoryAssets(string categoryId)
        {
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/assets/categories/" + categoryId + "/assets?page=1&pageSize=10000", "", tenanttoken);
            return ret;
        }
        public string getAssetCategoryById(string categoryId)
        {
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/assets/categories/" + categoryId, "", tenanttoken);
            return ret;
        }

        public string createAssetCategory(string assetType, string id, string name)
        {
            AssetCategory ac = new AssetCategory();
            ac.assetType = assetType;
            ac.id = id;
            ac.name = name;
            string data = JsonConvert.SerializeObject(ac);
            string ret = gp.HttpPost("http://" + httpurl + "/sitewhere/api/assets/categories", data, tenanttoken);
            return ret;
        }

        public string updateAssetCategory(string assetType, string id, string name)
        {
            AssetCategory ac = new AssetCategory();
            ac.assetType = assetType;
            ac.id = id;
            ac.name = name;
            string data = JsonConvert.SerializeObject(ac);
            string ret = gp.HttpPut("http://" + httpurl + "/sitewhere/api/assets/categories/" + id, data, tenanttoken);
            return ret;
        }

        public string getCategoryAsset(string categoryId, string assetId)
        {
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/assets/categories/" + categoryId + "/assets/" + assetId, "", tenanttoken);
            return ret;
        }

        public string createHardwareAsset(string categoryid, string id, string name, string imageUrl, string sku, string description, Dictionary<string, string> dic)
        {
            Assest assest = new Assest();
            assest.id = id;
            assest.name = name;
            assest.imageUrl = string.IsNullOrEmpty(imageUrl) ? "null" : imageUrl;
            assest.sku = sku;
            assest.description = description;
            assest.properties = dic;
            string data = JsonConvert.SerializeObject(assest);
            data = gp.HttpPost("http://" + httpurl + "/sitewhere/api/assets/categories/" + categoryid + "/hardware", data, tenanttoken);
            return data;
        }

        public string updateHardwareAsset(string categoryid, string id, string name, string imageUrl, string sku, string description, Dictionary<string, string> dic)
        {
            Assest assest = new Assest();
            assest.id = id;
            assest.name = name;
            assest.imageUrl = string.IsNullOrEmpty(imageUrl) ? "null" : imageUrl;
            assest.sku = sku;
            assest.description = description;
            assest.properties = dic;
            string data = JsonConvert.SerializeObject(assest);
            data = gp.HttpPut("http://" + httpurl + "/sitewhere/api/assets/categories/" + categoryid + "/hardware/" + id, data, tenanttoken);
            return data;
        }

        public string getDeviceSpecificationByToken(string token)
        {
            token = string.IsNullOrEmpty(token) ? Guid.NewGuid().ToString("N") : token;
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/specifications/" + token, "", tenanttoken);
            return ret;
        }

        public string createDeviceSpecification(string assetId, string assetModuleId, string name, string token)
        {
            token = string.IsNullOrEmpty(token) ? Guid.NewGuid().ToString(): token;
            Specifications specifications = new Specifications();
            specifications.assetId = assetId;
            specifications.assetModuleId = assetModuleId;
            specifications.name = name;
            specifications.token = token;
            specifications.containerPolicy = "Standalone";
            string data = JsonConvert.SerializeObject(specifications);
            string ret = gp.HttpPost("http://" + httpurl + "/sitewhere/api/specifications", data, tenanttoken);
            return ret;
        }

        public string updateDeviceSpecification(string assetId, string assetModuleId, string name, string token)
        {
            Specifications specifications = new Specifications();
            specifications.assetId = assetId;
            specifications.assetModuleId = assetModuleId;
            specifications.name = name;
            specifications.token = token;
            specifications.containerPolicy = "Standalone";
            string data = JsonConvert.SerializeObject(specifications);
            string ret = gp.HttpPut("http://" + httpurl + "/sitewhere/api/specifications/" + token, data, tenanttoken);
            return ret;
        }

        public string getDeviceByHardwareId(string hardwareId)
        {
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/devices/" + hardwareId, "", tenanttoken);
            return ret;
        }

        public string createDevice(string hardwareId, string siteToken, string specificationToken, Dictionary<string, string> dic)
        {
            Device d = new Device();
            d.hardwareId = hardwareId;
            d.siteToken = siteToken;
            d.specificationToken = specificationToken;
            d.comments = "";
            d.metadata = dic;
            string data = JsonConvert.SerializeObject(d);
            string ret = gp.HttpPost("http://" + httpurl + "/sitewhere/api/devices/", data, tenanttoken);
            return ret;
        }

        public string updateDevice(string hardwareId, string siteToken, string specificationToken, Dictionary<string, string> dic)
        {
            Device d = new Device();
            d.hardwareId = hardwareId;
            d.siteToken = siteToken;
            d.specificationToken = specificationToken;
            d.comments = "";
            d.metadata = dic;
            string data = JsonConvert.SerializeObject(d);
            string ret = gp.HttpPut("http://" + httpurl + "/sitewhere/api/devices/" + hardwareId, data, tenanttoken);
            return ret;
        }

        public string listDevices_excludeAssigned()
        {
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/devices?includeDeleted=false&excludeAssigned=true&includeSpecification=false&includeAssignment=false&page=1&pageSize=" + excludeAssignedPageSize, "", tenanttoken);
            return ret;
        }

        public string createDeviceAssignment(string deviceHardwareId, string assetModuleId, string assetId)
        {
            Assignments assignments = new Assignments();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            assignments.token = Guid.NewGuid().ToString("N");
            assignments.deviceHardwareId = deviceHardwareId;
            assignments.assignmentType = "Associated";
            assignments.assetModuleId = assetModuleId;
            assignments.assetId = assetId;
            assignments.metadata = dic;
            string data = JsonConvert.SerializeObject(assignments);
            data = gp.HttpPost("http://" + httpurl + "/sitewhere/api/assignments/", data, tenanttoken);
            string sql = "  update T_RTIOT_devices set S_assignmentsToken='" + assignments.token + "' where S_RTUID='" + deviceHardwareId + "'";
            DbHelperSQL.ExecuteSql(sql);
            return data;
        }


        public string getDeviceGroupByToken(string token)
        {
            token = string.IsNullOrEmpty(token) ? Guid.NewGuid().ToString("N") : token;
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/devicegroups/" + token, "", tenanttoken);
            return ret;
        }

        public string createDeviceGroup(string S_groupID, string name, string description, List<string> itemrole,Dictionary<string,string> dic)
        {
            Group2 g = new Group2();
            g.groupToken = Guid.NewGuid().ToString("N");
            g.name = name;
            g.description = description;
            g.roles = itemrole;
            g.metadata = dic;
            string data = JsonConvert.SerializeObject(g);
            string ret = gp.HttpPost("http://" + httpurl + "/sitewhere/api/devicegroups", data, tenanttoken);
            return ret;
        }

        public string updateDeviceGroup(string groupToken, string name, string description, List<string> itemrole,Dictionary<string,string> dic)
        {
            Group2 g = new Group2();
            g.groupToken = groupToken;
            g.name = name;
            g.description = description;
            g.roles = itemrole;
            g.metadata = dic;
            string data = JsonConvert.SerializeObject(g);
            string ret = gp.HttpPut("http://" + httpurl + "/sitewhere/api/devicegroups/" + groupToken, data, tenanttoken);
            return ret;
        }

        public string listDeviceGroups()
        {
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/devicegroups", "", tenanttoken);
            return ret;
        }
        public string listDeviceGroupElements(string groupToken)
        {
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/devicegroups/" + groupToken + "/elements?includeDetails=false&page=1&pageSize=100000", "", tenanttoken);
            return ret;
        }

        public string addDeviceGroupElements(List<Group> list_group, string groupToken)
        {
            string data = JsonConvert.SerializeObject(list_group);
            string ret = gp.HttpPut("http://" + httpurl + "/sitewhere/api/devicegroups/" + groupToken + "/elements", data, tenanttoken);
            string elementIds = "";
            foreach (Group item in list_group)
            {
                elementIds += "'"+item.elementId+"',";
                // UpdateFlag("T_RTIOT_groupelements", "S_groupToken='" + item.token + "' and S_RTUID_FK='" + item.elementId + "'");
            }
            if (!string.IsNullOrEmpty(elementIds))
            {
                elementIds = elementIds.Substring(0, elementIds.Length - 1);
                UpdateFlag("T_RTIOT_groupelements", "S_groupToken='" + groupToken + "' and S_RTUID_FK in (" + elementIds + ")");
            }
          
            return ret;
        }

        public string deleteDeviceGroupElement(string groupToken, string type, string elementId)
        {
            string ret = gp.HttpDELETE("http://" + httpurl + "/sitewhere/api/devicegroups/" + groupToken + "/elements/" + type + "/" + elementId, tenanttoken);
            return ret;
        }

        public string getDeviceCurrentAssignment(string hardwareId)
        {
            string ret = gp.HttpGet("http://" + httpurl + "/sitewhere/api/devices/" + hardwareId + "/assignment?includeAsset=false&includeDevice=false&includeSite=false", "", tenanttoken);
            return ret;
        }
    }
}