using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DTcms.Common
{
    /// <summary>
    /// 百度搜索引擎帮助类
    /// </summary>
    public class SeoHelper
    {
        /// <summary>
        ///直接将提供的Url发送到Ping百度http://ping.baidu.com/ping.html
        /// </summary>
        /// <param name="url">要发送的url注意带上http://</param>
        /// <returns>成功true 否则为False</returns>
        public static Boolean PingBaidu(string url)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\"?>");
                sb.Append("<methodCall>");
                sb.Append("<methodName>weblogUpdates.ping</methodName>");
                sb.Append("<params>");
                sb.Append("<param>");
                sb.Append("<value><string>" + url + "</string></value>");
                sb.Append("</param><param><value><string>" + url + "</string></value>");
                sb.Append("</param>");
                sb.Append("</params>");
                sb.Append("</methodCall>");
                HttpHelper http = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = "http://ping.baidu.com/ping/RPC2",//URL     必需项
                    Method = "POST",//URL     可选项 默认为Get
                    Referer = "http://ping.baidu.com/ping.html",//来源URL     可选项
                    Postdata = sb.ToString(),//Post数据     可选项GET时不需要写
                    ProtocolVersion = HttpVersion.Version10,
                };
                HttpResult result = http.GetHtml(item);
                if (result.Html.Contains("<int>0</int>"))
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
        /// <summary>
        /// 百度链接主动提交
        /// </summary>
        /// <param name="curl">地址</param>
        /// <param name="token">TOKEN</param>
        /// <param name="type">是否原创</param>
        /// <returns></returns>
        public static Boolean BaiduUrls(string curl, string token, bool type)
        {
            string url = "http://data.zz.baidu.com/urls?site={0}&token={1}";
            if (type)
            {
                url += "&type=original";
            }
            return SubmitBaidu(string.Format(url, new Uri(curl).Host, token), curl, token);
        }
        /// <summary>
        /// 百度链接更新
        /// </summary>
        /// <param name="curl">地址</param>
        /// <param name="token">TOKEN</param>
        /// <returns></returns>
        public static Boolean BaiduUpdate(string curl, string token)
        {
            return SubmitBaidu(string.Format("http://data.zz.baidu.com/update?site={0}&token={1}", new Uri(curl).Host, token), curl, token);
        }
        /// <summary>
        /// 百度链接删除
        /// </summary>
        /// <param name="curl">地址</param>
        /// <param name="token">TOKEN</param>
        /// <returns></returns>
        public static Boolean BaiduDel(string curl, string token)
        {
            return SubmitBaidu(string.Format("http://data.zz.baidu.com/del?site={0}&token={1}", new Uri(curl).Host, token), curl, token);
        }
        /// <summary>
        ///提交给百度接口
        /// </summary>
        private static Boolean SubmitBaidu(string url  ,string curl, string token)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,//URL     必需项
                Method = "POST",//URL     可选项 默认为Get
                Referer = curl,//来源URL     可选项
                Postdata = curl,//Post数据     可选项GET时不需要写
                ProtocolVersion = HttpVersion.Version10,
                ContentType = "text/plain",
                UserAgent = "curl/7.12.1"
            };
            HttpResult result = http.GetHtml(item);
            if (result.Html.Contains("\"success\":1"))
            {
                return true;
            }
            return false;
        }
    }
}
