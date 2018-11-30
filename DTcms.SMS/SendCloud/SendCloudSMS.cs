using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using DTcms.Common;

namespace DTcms.SMS.SendCloud
{
    public class SendCloudSMS
    {
        public static String SMS_api_url = "http://www.sendcloud.net/smsapi/send";
        public static String SMS_api_user = "";
        public static String SMS_api_key = "";


        public static String generate_md5(String str)
        {
            System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(str));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
        private static void LoadConfig()
        {
            string strXmlFile = HttpContext.Current.Server.MapPath("~/xmlconfig/sendcloud_sms.config");
            XmlControl XmlTool = new XmlControl(strXmlFile);
            SMS_api_user = XmlTool.GetText("Root/api_user");
            SMS_api_key = XmlTool.GetText("Root/api_key");
            XmlTool.Dispose();
        }
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="tel"></param>
        public static bool SendCode(string msg, string tel)
        {
            LoadConfig();

            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<String, String>("smsUser", SMS_api_user));
            paramList.Add(new KeyValuePair<String, String>("templateId", "11229"));//这个编号跟SendCloud短信模板id相呼应
            paramList.Add(new KeyValuePair<String, String>("phone", tel));
            paramList.Add(new KeyValuePair<String, String>("msgType", "0"));
            paramList.Add(new KeyValuePair<String, String>("vars", "{\"%code%\":\"" + msg + "\"}"));

            paramList.Sort(
                delegate(KeyValuePair<String, String> p1, KeyValuePair<String, String> p2)
                {
                    return p1.Key.CompareTo(p2.Key);
                }
            );

            var param_str = "";
            foreach (var param in paramList)
            {
                param_str += param.Key.ToString() + "=" + param.Value.ToString() + "&";
            }

            String sign_str = SMS_api_key + "&" + param_str + SMS_api_key;
            String sign = generate_md5(sign_str);

            paramList.Add(new KeyValuePair<String, String>("signature", sign));

            HttpClient client = null;
            HttpResponseMessage response = null;

            try
            {
                client = new HttpClient();
                response = client.PostAsync(SMS_api_url, new FormUrlEncodedContent(paramList)).Result;
                String result = response.Content.ReadAsStringAsync().Result;
                LogHelper.Error("result:" + result);
                return true;

            }
            catch (Exception e)
            {
                LogHelper.Error("Message : " + e.Message);
                return false;
            }
            finally
            {
                if (null != response)
                {
                    response.Dispose();
                }
                if (null != client)
                {
                    client.Dispose();
                }
            }
        }

        /// <summary>
        /// 客服通知
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tel"></param>
        /// <returns></returns>
        public static bool SendNotifyMessage(string type, string tel)
        {
            LoadConfig();
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<String, String>("smsUser", SMS_api_user));
            paramList.Add(new KeyValuePair<String, String>("templateId", "11097"));//这个编号跟SendCloud短信模板id相呼应
            paramList.Add(new KeyValuePair<String, String>("phone", tel));
            paramList.Add(new KeyValuePair<String, String>("msgType", "0"));
            paramList.Add(new KeyValuePair<String, String>("vars", "{\"%type%\":\"" + type + "\"}"));

            paramList.Sort(
                delegate(KeyValuePair<String, String> p1, KeyValuePair<String, String> p2)
                {
                    return p1.Key.CompareTo(p2.Key);
                }
            );

            var param_str = "";
            foreach (var param in paramList)
            {
                param_str += param.Key.ToString() + "=" + param.Value.ToString() + "&";
            }

            String sign_str = SMS_api_key + "&" + param_str + SMS_api_key;
            String sign = generate_md5(sign_str);

            paramList.Add(new KeyValuePair<String, String>("signature", sign));

            HttpClient client = null;
            HttpResponseMessage response = null;

            try
            {
                client = new HttpClient();
                response = client.PostAsync(SMS_api_url, new FormUrlEncodedContent(paramList)).Result;
                String result = response.Content.ReadAsStringAsync().Result;
                LogHelper.Error("result:" + result);
                return true;

            }
            catch (Exception e)
            {
                LogHelper.Error("Message : " + e.Message);
                return false;
            }
            finally
            {
                if (null != response)
                {
                    response.Dispose();
                }
                if (null != client)
                {
                    client.Dispose();
                }
            }
        }
    }
}
