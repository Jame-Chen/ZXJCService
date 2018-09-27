using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using log4net;
using System.Configuration;

namespace WcfServiceZXJC
{

    public class GetPost
    {

        ILog log = LogManager.GetLogger(typeof(GetPost));
        private string account = ConfigurationManager.AppSettings["account"];
        /// <summary>
        /// 后台发送POST请求
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="data">发送的数据</param>
        /// <returns></returns>
        public string HttpPost(string url, string data, string TenantID)
        {
            string value = "";
            HttpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                //创建post请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";
                byte[] bytes = Encoding.UTF8.GetBytes(account);
                string bb = Convert.ToBase64String(bytes);
                request.Headers.Add("Authorization:Basic " + bb);
                request.Headers.Add("X-Sitewhere-Tenant:" + TenantID);

                byte[] payload = Encoding.UTF8.GetBytes(data);
                request.ContentLength = payload.Length;


                //发送post的请求
                Stream writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
                writer.Dispose();


                //接受返回来的数据
                response = (HttpWebResponse)request.GetResponse();
                stream = response.GetResponseStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                value = reader.ReadToEnd();

                reader.Close();
                reader.Dispose();
                stream.Close();
                stream.Dispose();
                response.Close();
                response.Dispose();
            }
            catch (WebException e)
            {
                log.Error("出错啦:" + url + ",时间：" + DateTime.Now.ToString() + "，详细信息：" + e.Message);
                throw;
            }
            return value;
        }

        /// <summary>
        /// 后台发送GET请求
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="data">发送的数据</param>
        /// <returns></returns>
        public string HttpGet(string url, string data, string TenantID)
        {
            string retString = "";
            try
            {
                //创建Get请求
                url = url + (data == "" ? "" : "?") + data;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                byte[] bytes = Encoding.Default.GetBytes(account);
                string bb = Convert.ToBase64String(bytes);
                request.Headers.Add("Authorization:Basic " + bb);
                request.Headers.Add("X-Sitewhere-Tenant:" + TenantID);
                request.Headers.Set("Cache-Control", "no-cache");
                request.Headers.Set("Pragma", "no-cache");
                //接受返回来的数据

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                retString = streamReader.ReadToEnd();

                streamReader.Close();
                stream.Close();
                response.Close();


            }
            catch (Exception e)
            {
                log.Error("出错啦:" + url + ",时间：" + DateTime.Now.ToString() + "，详细信息：" + e.Message);
                return retString;
            }
            return retString;
        }

        /// <summary>
        /// 后台发送PUT请求
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="data">发送的数据</param>
        /// <returns></returns>
        public string HttpPut(string url, string data, string TenantID)
        {
            string value = "";
            HttpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                //创建post请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "PUT";
                request.ContentType = "application/json;charset=UTF-8";
                byte[] bytes = Encoding.UTF8.GetBytes(account);
                string bb = Convert.ToBase64String(bytes);
                request.Headers.Add("Authorization:Basic " + bb);
                request.Headers.Add("X-Sitewhere-Tenant:" + TenantID);

                byte[] payload = Encoding.UTF8.GetBytes(data);
                request.ContentLength = payload.Length;


                //发送post的请求
                Stream writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
                writer.Dispose();


                //接受返回来的数据
                response = (HttpWebResponse)request.GetResponse();
                stream = response.GetResponseStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                value = reader.ReadToEnd();

                reader.Close();
                reader.Dispose();
                stream.Close();
                stream.Dispose();
                response.Close();
                response.Dispose();
            }
            catch (WebException e)
            {
                log.Error("出错啦:" + url + ",时间：" + DateTime.Now.ToString() + "，详细信息：" + e.Message);
                throw;
            }
            return value;
        }

        /// <summary>
        /// 后台发送DELETE请求
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="data">发送的数据</param>
        /// <returns></returns>
        public string HttpDELETE(string url, string TenantID)
        {
            string value = "";
            HttpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                //创建post请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "DELETE";
                request.ContentType = "application/json;charset=UTF-8";
                byte[] bytes = Encoding.UTF8.GetBytes(account);
                string bb = Convert.ToBase64String(bytes);
                request.Headers.Add("Authorization:Basic " + bb);
                request.Headers.Add("X-Sitewhere-Tenant:" + TenantID);



                //接受返回来的数据
                response = (HttpWebResponse)request.GetResponse();
                stream = response.GetResponseStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                value = reader.ReadToEnd();

                reader.Close();
                reader.Dispose();
                stream.Close();
                stream.Dispose();
                response.Close();
                response.Dispose();
            }
            catch (WebException e)
            {
                log.Error("出错啦:" + url + ",时间：" + DateTime.Now.ToString() + "，详细信息：" + e.Message);
                throw;
            }
            return value;
        }
    }
}
