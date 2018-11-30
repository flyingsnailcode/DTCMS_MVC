using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Web;
using DTcms.Common;

namespace DTcms.SMS.SendCloud
{
    // 模板发送
    public class SendCloudEmail
    {
        public static String Email_api_user = "";
        public static String Email_api_key = "";
        public static String Email_fromname = "";
        public static String Email_from = "";
        private static void LoadConfig()
        {
            string strXmlFile = HttpContext.Current.Server.MapPath("~/xmlconfig/sendcloud_email.config");
            XmlControl XmlTool = new XmlControl(strXmlFile);
            Email_api_user = XmlTool.GetText("Root/api_user");
            Email_api_key = XmlTool.GetText("Root/api_key");
            Email_fromname = XmlTool.GetText("Root/fromname");
            Email_from = XmlTool.GetText("Root/from");
            XmlTool.Dispose();
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="_subject">标题</param>
        /// <param name="_templatename">模板代码</param>
        /// <param name="xsmtpapi"></param>
        /// <returns></returns>
        public static string Send(string _subject, string _templatename, String xsmtpapi)
        {
            String url = "http://api.sendcloud.net/apiv2/mail/sendtemplate";
            LoadConfig();
            
            HttpClient client = null;
            HttpResponseMessage response = null;

            try
            {
                #region
                client = new HttpClient();

                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();

                paramList.Add(new KeyValuePair<string, string>("apiUser", Email_api_user));
                paramList.Add(new KeyValuePair<string, string>("apiKey", Email_api_key));
                paramList.Add(new KeyValuePair<string, string>("from", Email_from));
                paramList.Add(new KeyValuePair<string, string>("fromname", Email_fromname));
                paramList.Add(new KeyValuePair<string, string>("xsmtpapi", xsmtpapi));
                paramList.Add(new KeyValuePair<string, string>("subject", _subject));
                paramList.Add(new KeyValuePair<string, string>("templateInvokeName", _templatename));

                response = client.PostAsync(url, new FormUrlEncodedContent(paramList)).Result;
                String result = response.Content.ReadAsStringAsync().Result;
                //LogHelper.Error(result);
                #endregion
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return ex.Message;
            }
            #region
            finally
            {
                if (null != client)
                {
                    client.Dispose();
                }
            }
            #endregion
        }
        /// <summary>
        /// 发送附件
        /// </summary>
        /// <param name="_subject"></param>
        /// <param name="_templatename"></param>
        /// <param name="xsmtpapi"></param>
        /// <param name="_filepath">物理路径，附件不超过10M</param>
        /// <param name="_filename">文件名</param>
        /// <returns></returns>
        public static string SendAttachments(string _subject, string _templatename, String xsmtpapi, string _filepath, string _filename)
        {
            String url = "http://sendcloud.sohu.com/webapi/mail.send_template.json";
            //String url = "http://api.sendcloud.net/apiv2/mail/sendtemplate";
            LoadConfig();

            HttpClient client = null;
            HttpResponseMessage response = null;
            String result = "";
            try
            {
                client = new HttpClient();

                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();

                paramList.Add(new KeyValuePair<string, string>("api_user", Email_api_user));
                paramList.Add(new KeyValuePair<string, string>("api_key", Email_api_key));
                paramList.Add(new KeyValuePair<string, string>("from", Email_from));
                paramList.Add(new KeyValuePair<string, string>("fromname", Email_fromname));
                paramList.Add(new KeyValuePair<string, string>("subject", _subject));
                paramList.Add(new KeyValuePair<string, string>("template_invoke_name", _templatename));
                paramList.Add(new KeyValuePair<string, string>("substitution_vars", xsmtpapi));


                var multipartFormDataContent = new MultipartFormDataContent();
                foreach (var keyValuePair in paramList)
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value), String.Format("\"{0}\"", keyValuePair.Key));
                }

                multipartFormDataContent.Add(new ByteArrayContent(System.IO.File.ReadAllBytes(_filepath)), "\"File\"", "\"" + _filename + "\"");

                response = client.PostAsync(url, multipartFormDataContent).Result;
                result = response.Content.ReadAsStringAsync().Result;

            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            finally
            {
                if (null != client)
                {
                    client.Dispose();
                }
            }
            return result;
        }
    }
}

