using System;
using System.Collections.Generic;
using System.Text;
using DTcms.Common;

namespace DTcms.API.OAuth
{
    public class weixin_helper
    {
        public weixin_helper()
        { }
        /// <summary>
        /// 获取调用接口凭证
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> get_access_token(string app_id, string app_key)
        {
            string send_url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", app_id, app_key);
            //发送并接受返回值
            string result = Utils.HttpGet(send_url);
            if (result.Contains("errcode"))
            {
                return null;
            }
            try
            {
                Dictionary<string, object> dic = JsonHelper.DataRowFromJSON(result);
                return dic;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 取得临时的Access Token
        /// </summary>
        /// <param name="app_id">client_id</param>
        /// <param name="app_key">client_secret</param>
        /// <param name="return_uri">redirect_uri</param>
        /// <param name="code">临时Authorization Code，官方提示2小时过期</param>
        /// <returns>Dictionary</returns>
        public static Dictionary<string, object> get_access_token(string app_id, string app_key, string return_uri, string code)
        {
            string send_url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + app_id + "&secret=" + app_key + "&code=" + code + "&grant_type=authorization_code";
            //发送并接受返回值
            string result = Utils.HttpGet(send_url);
            if (result.Contains("errcode"))
            {
                return null;
            }
            try
            {
                Dictionary<string, object> dic = JsonHelper.DataRowFromJSON(result);
                return dic;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取用户个人信息
        /// </summary>
        /// <param name="access_token">临时的Access Token</param>
        /// <param name="open_id">普通用户的标识，对当前开发者帐号唯一</param>
        /// <returns>JsonData</returns>
        public static Dictionary<string, object> get_info(string access_token, string open_id)
        {
            string send_url = "https://api.weixin.qq.com/sns/userinfo?access_token=" + access_token + "&openid=" + open_id;
            //发送并接受返回值
            string result = Utils.HttpGet(send_url);
            if (result.Contains("errcode"))
            {
                return null;
            }
            try
            {
                Dictionary<string, object> dic = JsonHelper.DataRowFromJSON(result);
                if (dic.Count > 0)
                {
                    return dic;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

    }
}
