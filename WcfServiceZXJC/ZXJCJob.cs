using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.ServiceModel;
using System.Data;
using System.IO;
using log4net.Config;
using System.Threading;
using Newtonsoft.Json;
using System.Configuration;
using DawnXZ.DBUtility;
using MongoDB.Driver.Builders;
using MongoDB.Driver;


namespace WcfServiceZXJC
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ZXJCJob
    {
        ILog logger;
        SiteWhereMethod sm = new SiteWhereMethod();
        Timer TenantTimer;
        Timer SiteTimer;
        Timer AssetCategoryTimer;
        Timer HardwareAssetTimer;
        Timer SpecificationTimer;
        Timer DeviceTimer;
        Timer AssignmentTimer;
        Timer DeviceGroupTimer;
        Timer GroupElementTimer;
        bool HardwareAssetFlag = true;//是否执行完该方法
        bool DeviceFlag = true;//是否执行完该方法
        bool AssignmentFlag = true;//是否执行完该方法
        bool GroupElementFlag = true;//是否执行完该方法
        private string tenantid;
        private int Period;
        public ZXJCJob()
        {
            tenantid = ConfigurationManager.AppSettings["tenantid"];
            Period = Convert.ToInt32(ConfigurationManager.AppSettings["Period"]);
            InitLog4Net();
            logger = LogManager.GetLogger(typeof(ZXJCJob));
            logger.Info("--------START---------");
            try
            {

                //Timer构造函数参数说明：
                //Callback：一个　TimerCallback 委托，表示要执行的方法。
                //State：一个包含回调方法要使用的信息的对象，或者为空引用（Visual　Basic 中为 Nothing）。
                //dueTime：调用　callback 之前延迟的时间量（以毫秒为单位）。指定 Timeout.Infinite 以防止计时器开始计时。指定零 (0) 以立即启动计时器。
                ////Period：调用　callback 的时间间隔（以毫秒为单位）。指定 Timeout.Infinite 可以禁用定期终止。
                TenantTimer = new Timer(new TimerCallback(TenantSynchronize), null, 10000, Period);
                SiteTimer = new Timer(new TimerCallback(SiteSynchronize), null, 10000, Period);
                AssetCategoryTimer = new Timer(new TimerCallback(AssetCategorySynchronize), null, 10000, Period);
                HardwareAssetTimer = new Timer(new TimerCallback(HardwareAssetSynchronize), null, 10000, Period);
                SpecificationTimer = new Timer(new TimerCallback(SpecificationSynchronize), null, 10000, Period);
                DeviceTimer = new Timer(new TimerCallback(DeviceSynchronize), null, 10000, Period);
                AssignmentTimer = new Timer(new TimerCallback(AssignmentSynchronize), null, 10000, Period);
                DeviceGroupTimer = new Timer(new TimerCallback(DeviceGroupSynchronize), null, 10000, Period);
                GroupElementTimer = new Timer(new TimerCallback(DeviceGroupElementSynchronize), null, 10000, Period);
               
            }
            catch (Exception e)
            {
                logger.Error("WCF服务异常" + e.Message);
            }
            logger.Info("--------END---------");
        }

        private static void InitLog4Net()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }


        /// <summary>
        /// 租户同步
        /// </summary>
        /// <param name="obj"></param>
        public void TenantSynchronize(object obj)
        {
            try
            {
                string sql = "select S_TenantID,S_TenantName,S_logoUrl,S_authenticationToken,S_authorizedUserIds from T_RTIOT_tenants where S_ISUSE='1' and S_UpdateFlag='1' and S_TenantID='" + tenantid + "'";
                DataTable dt = DbHelperSQL.Query(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    string ret = sm.getTenantById(dt.Rows[0]["S_TenantID"].ToString().Trim());
                    if (!string.IsNullOrEmpty(ret))
                    {
                        sm.updateTenant(dt.Rows[0]["S_TenantID"].ToString().Trim(), dt.Rows[0]["S_TenantName"].ToString(), dt.Rows[0]["S_logoUrl"].ToString(), dt.Rows[0]["S_authenticationToken"].ToString().Trim(), dt.Rows[0]["S_authorizedUserIds"].ToString());
                    }
                    else
                    {
                        sm.createTenant(dt.Rows[0]["S_TenantID"].ToString().Trim(), dt.Rows[0]["S_TenantName"].ToString(), dt.Rows[0]["S_logoUrl"].ToString(), dt.Rows[0]["S_authenticationToken"].ToString().Trim(), dt.Rows[0]["S_authorizedUserIds"].ToString());
                    }
                    sm.UpdateFlag("T_RTIOT_tenants", "S_TenantID='" + dt.Rows[0]["S_TenantID"].ToString().Trim() + "'");
                }
            }
            catch (Exception e)
            {
                logger.Error("TenantSynchronize异常:" + e.Message);

            }

        }

        /// <summary>
        /// 站点同步
        /// </summary>
        /// <param name="obj"></param>
        public void SiteSynchronize(object obj)
        {
            try
            {
                string sql = @"select S_SiteID,S_SiteName,S_SiteDes,S_SiteImageUrl,S_SiteToken,N_OrderBy  from T_RTIOT_sites where S_ISUSE='1' and S_UpdateFlag='1' and S_TenantID_FK='" + tenantid + "'";
                DataTable dt = DbHelperSQL.Query(sql).Tables[0];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string ret = sm.getSiteByToken(dt.Rows[i]["S_SiteToken"].ToString().Trim());
                    if (!string.IsNullOrEmpty(ret))
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("N_OrderBy", dt.Rows[i]["N_OrderBy"].ToString().Trim());
                        sm.updateSite(dt.Rows[i]["S_SiteToken"].ToString().Trim(), dt.Rows[i]["S_SiteImageUrl"].ToString(), dt.Rows[i]["S_SiteName"].ToString(), dt.Rows[i]["S_SiteDes"].ToString().Trim(), dic);
                    }
                    else
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("N_OrderBy", dt.Rows[i]["N_OrderBy"].ToString().Trim());
                        Sites s = JsonConvert.DeserializeObject<Sites>(sm.createSite(Guid.NewGuid().ToString("N"), dt.Rows[i]["S_SiteImageUrl"].ToString(), dt.Rows[i]["S_SiteName"].ToString(), dt.Rows[i]["S_SiteDes"].ToString().Trim(), dic));
                        sm.UpdateToken("T_RTIOT_sites", "S_SiteToken='" + s.token + "'", "S_SiteID='" + dt.Rows[i]["S_SiteID"].ToString().Trim() + "'");
                    }
                    sm.UpdateFlag("T_RTIOT_sites", "S_SiteID='" + dt.Rows[i]["S_SiteID"].ToString().Trim() + "'");


                }
            }
            catch (Exception e)
            {
                logger.Error("SiteSynchronize异常:" + e.Message);

            }

        }

        /// <summary>
        /// 资产类别同步
        /// </summary>
        /// <param name="obj"></param>
        public void AssetCategorySynchronize(object obj)
        {
            try
            {
                string sql = @"select S_AssetCategoryID,S_AssetCategoryN,b.S_NAME S_AssetType from dbo.T_RTIOT_assetCategorys a, dbo.T_RT_CONFIG b
                                       where a.S_AssetType=b.S_DIC_VALUE and b.s_dic_upno='S_AssetType-T_RTIOT_assets' and a.S_ISUSE='1' and S_UpdateFlag='1' and S_TenantID_FK='" + tenantid + "'";
                DataTable dt = DbHelperSQL.Query(sql).Tables[0];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string ret = sm.getAssetCategoryById(dt.Rows[i]["S_AssetCategoryID"].ToString().Trim());
                    if (!string.IsNullOrEmpty(ret))
                    {
                        sm.updateAssetCategory(dt.Rows[i]["S_AssetType"].ToString().Trim(), dt.Rows[i]["S_AssetCategoryID"].ToString(), dt.Rows[i]["S_AssetCategoryN"].ToString());
                    }
                    else
                    {
                        sm.createAssetCategory(dt.Rows[i]["S_AssetType"].ToString().Trim(), dt.Rows[i]["S_AssetCategoryID"].ToString(), dt.Rows[i]["S_AssetCategoryN"].ToString());
                    }
                    sm.UpdateFlag("T_RTIOT_assetCategorys", "S_AssetCategoryID='" + dt.Rows[i]["S_AssetCategoryID"].ToString().Trim() + "'");
                }
            }
            catch (Exception e)
            {
                logger.Error("AssetCategorySynchronize异常:" + e.Message);

            }
        }

        /// <summary>
        /// 资产同步
        /// </summary>
        /// <param name="obj"></param>
        public void HardwareAssetSynchronize(object obj)
        {
            if (!HardwareAssetFlag)
            {
                return;
            }
            HardwareAssetFlag = false;
            try
            {
                string sql = @" select a.S_AssetCategoryID,a.S_AssetImageUrl,S_ADD,a.S_STID,a.S_Name,S_extno,b.s_name s_type_id ,c.s_name s_dist_id,S_Manage_Unit,a.N_OrderBy,a.S_DRAINAGESYS,a.S_ISDEL,S_RingRoad,
                                        N_FXTCSW,N_FXKCSW,N_JYTCSW ,N_JYKCSW,N_JLJYKCSW,N_JLJYTCSW,N_JLHLKCSW,N_JLHLTCSW
                                        
                                        from dbo.T_RTIOT_assets a
                                        left join T_RT_CONFIG b on right(a.s_type_id,3)=b.S_dic_value and b.s_dic_upno='S_TYPE_ID01-T_RTIOT_assets'
                                        left join T_RT_CONFIG c on a.s_dist_id=c.S_dic_value and c.s_dic_upno='QX-T_RTIOT_assets'
                                       
                                        where a.S_ISUSE='1' and a.S_UpdateFlag='1'";
                //N_JiangYuKaiCheShuiWei,N_JiangYuTingCheShuiWei,N_FangXunKaiCheShuiWei ,N_FangXunTingCheShuiWei
                //     left join (SELECT N_JiangYuKaiCheShuiWei,N_JiangYuTingCheShuiWei,N_FangXunKaiCheShuiWei ,N_FangXunTingCheShuiWei,S_XTBM  FROM T_DRAINPUMP) d on a.S_extno=d.S_XTBM
                DataTable dt = DbHelperSQL.Query(sql).Tables[0];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string ret = sm.getCategoryAsset(dt.Rows[i]["S_AssetCategoryID"].ToString().Trim(), dt.Rows[i]["S_STID"].ToString().Trim());
                    if (!string.IsNullOrEmpty(ret))
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        if (dt.Rows[i]["S_AssetCategoryID"].ToString().Trim() != "Specification" && dt.Rows[i]["S_AssetCategoryID"].ToString().Trim() != "DataSource")
                        {
                            dic.Add("S_EXTNO", dt.Rows[i]["S_EXTNO"].ToString());
                            dic.Add("S_TYPE_ID", dt.Rows[i]["S_TYPE_ID"].ToString());
                            dic.Add("S_DIST_ID", dt.Rows[i]["S_DIST_ID"].ToString());
                            dic.Add("S_Manage_Unit", dt.Rows[i]["S_Manage_Unit"].ToString());
                            dic.Add("N_OrderBy", dt.Rows[i]["N_OrderBy"].ToString());
                            dic.Add("S_DRAINAGESYS", dt.Rows[i]["S_DRAINAGESYS"].ToString());
                            dic.Add("S_ISDEL", dt.Rows[i]["S_ISDEL"].ToString());
                            dic.Add("S_RingRoad", dt.Rows[i]["S_RingRoad"].ToString());
                            dic.Add("N_FXTCSW", dt.Rows[i]["N_FXTCSW"].ToString());
                            dic.Add("N_FXKCSW", dt.Rows[i]["N_FXKCSW"].ToString());
                            dic.Add("N_JYTCSW", dt.Rows[i]["N_JYTCSW"].ToString());
                            dic.Add("N_JYKCSW", dt.Rows[i]["N_JYKCSW"].ToString());
                            dic.Add("N_JLJYKCSW", dt.Rows[i]["N_JLJYKCSW"].ToString());
                            dic.Add("N_JLJYTCSW", dt.Rows[i]["N_JLJYTCSW"].ToString());
                            dic.Add("N_JLHLKCSW", dt.Rows[i]["N_JLHLKCSW"].ToString());
                            dic.Add("N_JLHLTCSW", dt.Rows[i]["N_JLHLTCSW"].ToString());
                            //dic.Add("N_JiangYuKaiCheShuiWei", dt.Rows[i]["N_JiangYuKaiCheShuiWei"].ToString());
                            //dic.Add("N_JiangYuTingCheShuiWei", dt.Rows[i]["N_JiangYuTingCheShuiWei"].ToString());
                            //dic.Add("N_FangXunKaiCheShuiWei", dt.Rows[i]["N_FangXunKaiCheShuiWei"].ToString());
                            //dic.Add("N_FangXunTingCheShuiWei", dt.Rows[i]["N_FangXunTingCheShuiWei"].ToString());
                        }
                        if (dt.Rows[i]["S_AssetCategoryID"].ToString().Trim() == "DataSource")
                        {
                            dic.Add("N_OrderBy", dt.Rows[i]["N_OrderBy"].ToString());
                        }
                        sm.updateHardwareAsset(dt.Rows[i]["S_AssetCategoryID"].ToString().Trim(), dt.Rows[i]["S_STID"].ToString().Trim(), dt.Rows[i]["S_Name"].ToString().Trim(), dt.Rows[i]["S_AssetImageUrl"].ToString().Trim(), dt.Rows[i]["S_STID"].ToString().Trim(), dt.Rows[i]["S_ADD"].ToString().Trim(), dic);
                    }
                    else
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        if (dt.Rows[i]["S_AssetCategoryID"].ToString().Trim() != "Specification" && dt.Rows[i]["S_AssetCategoryID"].ToString().Trim() != "DataSource")
                        {
                            dic.Add("S_EXTNO", dt.Rows[i]["S_EXTNO"].ToString());
                            dic.Add("S_TYPE_ID", dt.Rows[i]["S_TYPE_ID"].ToString());
                            dic.Add("S_DIST_ID", dt.Rows[i]["S_DIST_ID"].ToString());
                            dic.Add("S_Manage_Unit", dt.Rows[i]["S_Manage_Unit"].ToString());
                            dic.Add("N_OrderBy", dt.Rows[i]["N_OrderBy"].ToString());
                            dic.Add("S_DRAINAGESYS", dt.Rows[i]["S_DRAINAGESYS"].ToString());
                            dic.Add("S_ISDEL", dt.Rows[i]["S_ISDEL"].ToString());
                            dic.Add("S_RingRoad", dt.Rows[i]["S_RingRoad"].ToString());
                            dic.Add("N_FXTCSW", dt.Rows[i]["N_FXTCSW"].ToString());
                            dic.Add("N_FXKCSW", dt.Rows[i]["N_FXKCSW"].ToString());
                            dic.Add("N_JYTCSW", dt.Rows[i]["N_JYTCSW"].ToString());
                            dic.Add("N_JYKCSW", dt.Rows[i]["N_JYKCSW"].ToString());
                            dic.Add("N_JLJYKCSW", dt.Rows[i]["N_JLJYKCSW"].ToString());
                            dic.Add("N_JLJYTCSW", dt.Rows[i]["N_JLJYTCSW"].ToString());
                            dic.Add("N_JLHLKCSW", dt.Rows[i]["N_JLHLKCSW"].ToString());
                            dic.Add("N_JLHLTCSW", dt.Rows[i]["N_JLHLTCSW"].ToString());
                            //dic.Add("N_JiangYuKaiCheShuiWei", dt.Rows[i]["N_JiangYuKaiCheShuiWei"].ToString());
                            //dic.Add("N_JiangYuTingCheShuiWei", dt.Rows[i]["N_JiangYuTingCheShuiWei"].ToString());
                            //dic.Add("N_FangXunKaiCheShuiWei", dt.Rows[i]["N_FangXunKaiCheShuiWei"].ToString());
                            //dic.Add("N_FangXunTingCheShuiWei", dt.Rows[i]["N_FangXunTingCheShuiWei"].ToString());
                        }
                        if (dt.Rows[i]["S_AssetCategoryID"].ToString().Trim() == "DataSource")
                        {
                            dic.Add("N_OrderBy", dt.Rows[i]["N_OrderBy"].ToString());
                        }
                        sm.createHardwareAsset(dt.Rows[i]["S_AssetCategoryID"].ToString().Trim(), dt.Rows[i]["S_STID"].ToString().Trim(), dt.Rows[i]["S_Name"].ToString().Trim(), dt.Rows[i]["S_AssetImageUrl"].ToString().Trim(), dt.Rows[i]["S_STID"].ToString().Trim(), dt.Rows[i]["S_ADD"].ToString().Trim(), dic);
                    }
                    sm.UpdateFlag("T_RTIOT_assets", "S_STID='" + dt.Rows[i]["S_STID"].ToString().Trim() + "'");
                }
            }
            catch (Exception e)
            {
                logger.Error("HardwareAssetSynchronize异常:" + e.Message);

            }
            finally
            {
                HardwareAssetFlag = true;
            }
        }

        /// <summary>
        /// 规格同步
        /// </summary>
        /// <param name="obj"></param>
        public void SpecificationSynchronize(object obj)
        {
            try
            {
                string sql = @"  select S_specID,S_AssetID_FK,S_AssetModuleID_FK,S_specN,S_specToken from T_RTIOT_specifications where S_ISUSE='1' and S_UpdateFlag='1' and S_TenantID='" + tenantid + "'";
                DataTable dt = DbHelperSQL.Query(sql).Tables[0];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (sm.getCategoryAsset(dt.Rows[i]["S_AssetModuleID_FK"].ToString().Trim(), dt.Rows[i]["S_AssetID_FK"].ToString().Trim()) != "")
                    {


                        string ret = sm.getDeviceSpecificationByToken(dt.Rows[i]["S_specToken"].ToString().Trim());
                        if (!string.IsNullOrEmpty(ret))
                        {
                            sm.updateDeviceSpecification(dt.Rows[i]["S_AssetID_FK"].ToString().Trim(), dt.Rows[i]["S_AssetModuleID_FK"].ToString().Trim(), dt.Rows[i]["S_specN"].ToString().Trim(), dt.Rows[i]["S_specToken"].ToString().Trim());
                        }
                        else
                        {
                            Specifications s = JsonConvert.DeserializeObject<Specifications>(sm.createDeviceSpecification(dt.Rows[i]["S_AssetID_FK"].ToString().Trim(), dt.Rows[i]["S_AssetModuleID_FK"].ToString().Trim(), dt.Rows[i]["S_specN"].ToString().Trim(), Guid.NewGuid().ToString("N")));
                            sm.UpdateToken("T_RTIOT_specifications", "S_specToken='" + s.token + "'", "S_specID='" + dt.Rows[i]["S_specID"].ToString().Trim() + "'");
                        }
                        sm.UpdateFlag("T_RTIOT_specifications", "S_specID='" + dt.Rows[i]["S_specID"].ToString().Trim() + "'");
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("SpecificationSynchronize异常:" + e.Message);

            }
        }

        /// <summary>
        /// 设备同步
        /// </summary>
        /// <param name="obj"></param>
        public void DeviceSynchronize(object obj)
        {
            if (!DeviceFlag)
            {
                return;
            }
            DeviceFlag = false;
            try
            {
                string sql = @" select b.S_specToken,g.S_SiteToken sitetoken,a.S_RTUID,S_RTUNAME,c.s_name N_RTUTYPE,d.s_name S_STID_FK,N_NOS_TIME,S_MODEL,e.s_name N_Sources_ID,
                                        S_MAINTAIN_UNIT,S_FILE_ID,a.N_OrderBy,f.s_name S_STATE, S_STATE_Run,S_offsetX,S_offsetY,S_ProjectNum,S_Manufacturers,Power,d. N_LGTD,d.N_LTTD,d.S_EXTNO,
                                        ( select * from GetTag(a.S_RTU_DES)) Tag
                                        from dbo.T_RTIOT_devices a
                                        right join (select S_specID,S_specToken from dbo.T_RTIOT_specifications) b on a.S_specID_FK=b.S_specID
                                        left join (select * from dbo.T_RT_CONFIG where s_dic_upno='N_RTUTYPE_01-T_RTIOT_devices') c on a.N_RTUTYPE=c.S_DIC_VALUE
                                        left join (select s_stid,s_name,N_LGTD,N_LTTD,S_EXTNO,S_MAINTAIN_UNIT from dbo.T_RTIOT_assets) d on a.S_STID_FK=d.S_STID
                                        left join (select * from dbo.T_RT_CONFIG where s_dic_upno='S_DSTYPE01-T_RTIOT_devices') e on e.s_no=a.N_Sources_ID
                                        left join (select * from dbo.T_RT_CONFIG where s_dic_upno='S_STATE01') f on a.S_STATE=f.S_DIC_VALUE
                                        right join (select S_SiteID,S_SiteToken from T_RTIOT_sites) g on a.S_SiteID_FK=g.S_SiteID where a.S_ISUSE='1' and a.S_UpdateFlag='1' and a.S_TenantID_FK='" + tenantid + "'";
                DataTable dt = DbHelperSQL.Query(sql).Tables[0];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string ret = sm.getDeviceByHardwareId(dt.Rows[i]["S_RTUID"].ToString().Trim());
                    if (!string.IsNullOrEmpty(ret))
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("S_EXTNO", dt.Rows[i]["S_EXTNO"].ToString());
                        dic.Add("S_RTUNAME", dt.Rows[i]["S_RTUNAME"].ToString());
                        dic.Add("N_RTUTYPE", dt.Rows[i]["N_RTUTYPE"].ToString());
                        dic.Add("S_STID_FK", dt.Rows[i]["S_STID_FK"].ToString());
                        dic.Add("N_NOS_TIME", dt.Rows[i]["N_NOS_TIME"].ToString());
                        dic.Add("S_MODEL", dt.Rows[i]["S_MODEL"].ToString());
                        dic.Add("N_Sources_ID", dt.Rows[i]["N_Sources_ID"].ToString());
                        dic.Add("S_FILE_ID", dt.Rows[i]["S_FILE_ID"].ToString());
                        dic.Add("N_OrderBy", dt.Rows[i]["N_OrderBy"].ToString());
                        dic.Add("S_STATE", dt.Rows[i]["S_STATE"].ToString());
                        dic.Add("S_STATE_Run", dt.Rows[i]["S_STATE_Run"].ToString());
                        dic.Add("S_offsetX", dt.Rows[i]["S_offsetX"].ToString());
                        dic.Add("S_offsetY", dt.Rows[i]["S_offsetY"].ToString());
                        dic.Add("S_ProjectNum", dt.Rows[i]["S_ProjectNum"].ToString());
                        dic.Add("S_Manufacturers", dt.Rows[i]["S_Manufacturers"].ToString());
                        dic.Add("Power", dt.Rows[i]["Power"].ToString());
                        dic.Add("N_LGTD", dt.Rows[i]["N_LGTD"].ToString());
                        dic.Add("N_LTTD", dt.Rows[i]["N_LTTD"].ToString());
                        dic.Add("S_MAINTAIN_UNIT", dt.Rows[i]["S_MAINTAIN_UNIT"].ToString());
                        dic.Add("Tag", dt.Rows[i]["Tag"].ToString());//入河排污口需要用到的字段
                        sm.updateDevice(dt.Rows[i]["S_RTUID"].ToString().Trim(), dt.Rows[i]["sitetoken"].ToString().Trim(), dt.Rows[i]["S_specToken"].ToString().Trim(), dic);
                    }
                    else
                    {

                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("S_EXTNO", dt.Rows[i]["S_EXTNO"].ToString());
                        dic.Add("S_RTUNAME", dt.Rows[i]["S_RTUNAME"].ToString());
                        dic.Add("N_RTUTYPE", dt.Rows[i]["N_RTUTYPE"].ToString());
                        dic.Add("S_STID_FK", dt.Rows[i]["S_STID_FK"].ToString());
                        dic.Add("N_NOS_TIME", dt.Rows[i]["N_NOS_TIME"].ToString());
                        dic.Add("S_MODEL", dt.Rows[i]["S_MODEL"].ToString());
                        dic.Add("N_Sources_ID", dt.Rows[i]["N_Sources_ID"].ToString());
                        dic.Add("S_FILE_ID", dt.Rows[i]["S_FILE_ID"].ToString());
                        dic.Add("N_OrderBy", dt.Rows[i]["N_OrderBy"].ToString());
                        dic.Add("S_STATE", dt.Rows[i]["S_STATE"].ToString());
                        dic.Add("S_STATE_Run", dt.Rows[i]["S_STATE_Run"].ToString());
                        dic.Add("S_offsetX", dt.Rows[i]["S_offsetX"].ToString());
                        dic.Add("S_offsetY", dt.Rows[i]["S_offsetY"].ToString());
                        dic.Add("S_ProjectNum", dt.Rows[i]["S_ProjectNum"].ToString());
                        dic.Add("S_Manufacturers", dt.Rows[i]["S_Manufacturers"].ToString());
                        dic.Add("Power", dt.Rows[i]["Power"].ToString());
                        dic.Add("N_LGTD", dt.Rows[i]["N_LGTD"].ToString());
                        dic.Add("N_LTTD", dt.Rows[i]["N_LTTD"].ToString());
                        dic.Add("S_MAINTAIN_UNIT", dt.Rows[i]["S_MAINTAIN_UNIT"].ToString());
                        dic.Add("Tag", dt.Rows[i]["Tag"].ToString());//入河排污口需要用到的字段
                        sm.createDevice(dt.Rows[i]["S_RTUID"].ToString().Trim(), dt.Rows[i]["sitetoken"].ToString().Trim(), dt.Rows[i]["S_specToken"].ToString().Trim(), dic);
                    }
                    sm.UpdateFlag("T_RTIOT_devices", "S_RTUID='" + dt.Rows[i]["S_RTUID"].ToString().Trim() + "'");
                }
            }
            catch (Exception e)
            {
                logger.Error("DeviceSynchronize异常:" + e.Message);

            }
            finally
            {
                DeviceFlag = true;
            }
        }

        public void AssignmentSynchronize(object obj)
        {
            if (!AssignmentFlag)
            {
                return;
            }
            AssignmentFlag = false;
            try
            {
                DeviceResults dr = JsonConvert.DeserializeObject<DeviceResults>(sm.listDevices_excludeAssigned());
                if (dr.numResults == "0")
                {
                    AssignmentFlag = true;
                    return;
                }
                List<Assest> list_assest = new List<Assest>();
                AssetCategoryResults acr = JsonConvert.DeserializeObject<AssetCategoryResults>(sm.listAssetCategories());
                foreach (AssetCategory atem in acr.results)
                {
                    if (atem.id != "Specification")
                    {
                        AssestResults ar = JsonConvert.DeserializeObject<AssestResults>(sm.listCategoryAssets(atem.id));
                        list_assest.AddRange(ar.results);
                    }
                }

                foreach (Device item in dr.results)
                {
                    int index = item.hardwareId.IndexOf("_");
                    string assetId = item.hardwareId.Substring(0, index);
                    Assest assest = list_assest.Find(f => f.id == assetId);
                    sm.createDeviceAssignment(item.hardwareId, assest.assetCategoryId, assest.id);
                }
            }
            catch (Exception e)
            {

                logger.Error("AssignmentSynchronize异常:" + e.Message);

            }
            finally
            {
                AssignmentFlag = true;
            }
        }

        /// <summary>
        /// 设备组同步
        /// </summary>
        /// <param name="obj"></param>
        public void DeviceGroupSynchronize(object obj)
        {
            try
            {
                string sql = @"select S_groupID,S_groupN,S_groupDes,S_groupToken,S_groupRoles,N_OrderBy from T_RTIOT_devicegroups where S_ISUSE='1' and S_UpdateFlag='1' and S_TenantID_FK='" + tenantid + "'";
                DataTable dt = DbHelperSQL.Query(sql).Tables[0];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string ret = sm.getDeviceGroupByToken(dt.Rows[i]["S_groupToken"].ToString().Trim());
                    List<string> role = dt.Rows[i]["S_groupRoles"].ToString().Trim().Split('|').ToList();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("TabPage", dt.Rows[i]["S_groupID"].ToString().Trim());
                    dic.Add("N_OrderBy", dt.Rows[i]["N_OrderBy"].ToString().Trim());
                    if (!string.IsNullOrEmpty(ret))
                    {
                        sm.updateDeviceGroup(dt.Rows[i]["S_groupToken"].ToString().Trim(), dt.Rows[i]["S_groupN"].ToString().Trim(), dt.Rows[i]["S_groupDes"].ToString().Trim(), role, dic);
                    }
                    else
                    {
                        Group2 g = JsonConvert.DeserializeObject<Group2>(sm.createDeviceGroup(dt.Rows[i]["S_groupID"].ToString().Trim(), dt.Rows[i]["S_groupN"].ToString().Trim(), dt.Rows[i]["S_groupDes"].ToString().Trim(), role, dic));
                        sm.UpdateToken("T_RTIOT_devicegroups", "S_groupToken='" + g.token + "'", "S_groupID='" + dt.Rows[i]["S_groupID"].ToString().Trim() + "'");
                    }
                    sm.UpdateFlag("T_RTIOT_devicegroups", "S_groupID='" + dt.Rows[i]["S_groupID"].ToString().Trim() + "'");
                }
            }
            catch (Exception e)
            {
                logger.Error("DeviceGroupSynchronize异常:" + e.Message);

            }
        }

        /// <summary>
        /// 设备组元素同步
        /// </summary>
        public void DeviceGroupElementSynchronize(object obj)
        {
            if (!GroupElementFlag)
            {
                return;
            }
            GroupElementFlag = false;
            try
            {
                GroupResults gr = JsonConvert.DeserializeObject<GroupResults>(sm.listDeviceGroups());
                foreach (Group item in gr.results)
                {
                    GroupResults grElements = JsonConvert.DeserializeObject<GroupResults>(sm.listDeviceGroupElements(item.token));
                    List<Group> a = new List<Group>();//sitewhere上设备组的设备
                    a.AddRange(grElements.results);

                    string sql = @"  select S_groupID_FK,S_groupToken,S_Roles,S_RTUID_FK,b.s_name from T_RTIOT_groupelements a,T_RT_CONFIG b 
                                              where a.S_AssetType=b.S_DIC_VALUE and b.S_DIC_UPNO='S_AssetType-T_RTIOT_assets'   and S_groupToken='" + item.token + "' and a.S_ISUSE='1' and a.S_UpdateFlag='1' and a.S_TenantID_FK='" + tenantid + "'";
                    DataTable dt = DbHelperSQL.Query(sql).Tables[0];
                    List<Group> b = new List<Group>();//数据库该设备组需要更新的设备
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Group gp = new Group();
                        gp.type = dt.Rows[i]["s_name"].ToString();
                        gp.roles = dt.Rows[i]["S_Roles"].ToString().Split('|').ToList();
                        gp.elementId = dt.Rows[i]["S_RTUID_FK"].ToString();
                        gp.groupToken = dt.Rows[i]["S_groupToken"].ToString();
                        b.Add(gp);
                    }

                    sql = @" select S_groupID_FK,S_groupToken,S_Roles,S_RTUID_FK,b.s_name from T_RTIOT_groupelements a,T_RT_CONFIG b 
                                   where a.S_AssetType=b.S_DIC_VALUE and b.S_DIC_UPNO='S_AssetType-T_RTIOT_assets'   and S_groupToken='" + item.token + "' and a.S_ISUSE='1' and a.S_TenantID_FK='" + tenantid + "'";
                    dt = DbHelperSQL.Query(sql).Tables[0];
                    List<Group> c = new List<Group>();//数据库该设备组所有的设备
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Group gp = new Group();
                        gp.type = dt.Rows[i]["s_name"].ToString();
                        gp.roles = dt.Rows[i]["S_Roles"].ToString().Split('|').ToList();
                        gp.elementId = dt.Rows[i]["S_RTUID_FK"].ToString();
                        gp.groupToken = dt.Rows[i]["S_groupToken"].ToString();
                        c.Add(gp);
                    }

                    List<Group> d = new List<Group>();//sitewhere需要删除的设备
                    foreach (Group atem in a)
                    {
                        if (b.Select(s => s.elementId).Contains(atem.elementId))
                        {
                            d.Add(atem);
                        }
                        if (!c.Select(s => s.elementId).Contains(atem.elementId))
                        {
                            if (atem.type != "Group")
                            {
                                d.Add(atem);
                            }
                        }
                    }
                    //   d.AddRange(a.Intersect(b).ToList());
                    //  d.AddRange(a.Except(c).ToList());
                    foreach (Group dtem in d)
                    {
                        sm.deleteDeviceGroupElement(dtem.groupToken, dtem.type, dtem.elementId);
                    }
                    sm.addDeviceGroupElements(b, item.token);

                }
            }
            catch (Exception e)
            {
                logger.Error("DeviceGroupElementSynchronize异常:" + e.Message);

            }
            finally
            {
                GroupElementFlag = true;
            }
        }

        public void UpdateAssignment()
        {
            string sql = "select S_RTUID from T_RTIOT_devices where S_assignmentsToken is null ";
            DataTable dt = DbHelperSQL.Query(sql).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Assignments ass = JsonConvert.DeserializeObject<Assignments>(sm.getDeviceCurrentAssignment(dt.Rows[i]["S_RTUID"].ToString().Trim()));
                if (ass != null)
                {
                    sql = "update T_RTIOT_devices set S_assignmentsToken='" + ass.token + "' where S_RTUID='" + dt.Rows[i]["S_RTUID"].ToString().Trim() + "'";
                    DbHelperSQL.ExecuteSql(sql);
                }

            }
        }

        public void UpdateDevice()
        {
            MongoDBHandler handler = new MongoDBHandler("mongodb://172.18.1.221:27017", "tenant-zxjc");
            MongoDBHelperAt md = new MongoDBHelperAt(handler);
            //IMongoQuery mq = Query.And(Query.EQ("hardwareId", "010701001_L1"));
            List<Device> list = md.Find<Device>(null, "devices");
            List<string> hardwares = list.Select(s => s.hardwareId).ToList();
            string sql = "select S_RTUID from T_RTIOT_devices";
            DataTable dt = DbHelperSQL.Query(sql).Tables[0];
            List<string> sqlhardwares = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sqlhardwares.Add(dt.Rows[i]["S_RTUID"].ToString());
            }
            List<string> chaji = hardwares.Except(sqlhardwares).ToList();
            foreach (string item in chaji)
            {
                IMongoQuery imq = Query.And(Query.EQ("hardwareId", item));
                md.Remove<Device>(imq, "devices");
            }


        }
    }
}