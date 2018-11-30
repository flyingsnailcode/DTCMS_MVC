using System;
using System.Collections.Generic;
using System.Text;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;

namespace DTcms.API.Weixin.Common
{
    /// <summary>
    /// 负责获取或刷新AccessToken
    /// </summary>
    public class CRMComm
    {
        public CRMComm()
        { }

        BLL.weixin_access_token tokenBLL = new BLL.weixin_access_token(); //账户AccessToken
        BLL.weixin_account accountBLL = new BLL.weixin_account(); //公众平台账户

        /// <summary>
        /// 及时获得access_token值
        /// access_token是公众号的全局唯一票据，公众号调用各接口时都需使用access_token。正常情况下access_token有效期为7200秒，
        /// 重复获取将导致上次获取的access_token失效。
        /// 每日限额获取access_token.我们将access_token保存到数据库里，间隔时间为20分钟，从微信公众平台获得一次。
        /// </summary>
        public string GetAccessToken(int accountId, out string error)
        {
            string access_token = string.Empty;
            error = string.Empty;
            try
            {
                Model.weixin_account accountModel = accountBLL.GetModel(accountId); //公众平台账户信息
                if (string.IsNullOrEmpty(accountModel.appid) || string.IsNullOrEmpty(accountModel.appsecret))
                {
                    error = "AppId或者AppSecret未填写,请在补全信息！";
                    return string.Empty;
                }
                //没有找到该账户则获取AccessToKen写入存储1200秒
                if (!tokenBLL.ExistsAccount(accountId))
                {
                    var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetToken(accountModel.appid, accountModel.appsecret);
                    access_token = result.access_token;
                    tokenBLL.Add(accountId, access_token);
                    return access_token;
                }
                //获取公众账户的实体
                Model.weixin_access_token tokenModel = tokenBLL.GetAccountModel(accountId);
                //计算时间判断是否过期
                TimeSpan ts = DateTime.Now - tokenModel.add_time;
                double chajunSecond = ts.TotalSeconds;
                if (string.IsNullOrEmpty(tokenModel.access_token) || chajunSecond >= tokenModel.expires_in)
                {
                    //从微信平台重新获得AccessToken
                    var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetToken(accountModel.appid, accountModel.appsecret);
                    access_token = result.access_token;
                    //更新到数据库里的AccessToken
                    tokenModel.access_token = access_token;
                    tokenModel.add_time = DateTime.Now;
                    tokenBLL.Update(tokenModel);

                }
                else
                {
                    access_token = tokenModel.access_token;
                }
            }
            catch (Exception ex)
            {
                error = "获取AccessToken出错:" + ex.Message;
            }
            return access_token;
        }

        public string getSignature(string jsapi_ticket, string nonceStr, string timestamp, string url)
        {
            Senparc.Weixin.MP.Helpers.JSSDKHelper helper = new Senparc.Weixin.MP.Helpers.JSSDKHelper();
            string sign = helper.GetSignature(jsapi_ticket, nonceStr, timestamp, url);
            return sign;
        }

        /// <summary>
        /// 获取JSAPI_TICKET接口
        /// </summary>
        /// <returns></returns>
        public string GetJSAPI_Ticket(int accountId, string accessToken)
        {
            Model.weixin_account accountModel = accountBLL.GetModel(accountId); //公众平台账户信息
            string ticket = JsApiTicketContainer.TryGetTicket(accountModel.appid, accountModel.appsecret);
            return ticket;
        }

        /// <summary>
        ///【强制刷新】access_token值
        /// access_token是公众号的全局唯一票据，公众号调用各接口时都需使用access_token。正常情况下access_token有效期为7200秒，
        /// 重复获取将导致上次获取的access_token失效。
        /// 每日限额获取access_token.我们将access_token保存到数据库里，间隔时间为20分钟，从微信公众平台获得一次。
        /// </summary>
        /// <returns></returns>
        public string FlushAccessToken(int accountId, out string error)
        {
            string access_token = string.Empty;
            error = string.Empty;
            try
            {
                Model.weixin_account accountModel = accountBLL.GetModel(accountId); //公众平台账户信息
                if (string.IsNullOrEmpty(accountModel.appid) || string.IsNullOrEmpty(accountModel.appsecret))
                {
                    error = "AppId或者AppSecret未填写,请在补全信息！";
                    return "";
                }

                var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetToken(accountModel.appid, accountModel.appsecret);
                access_token = result.access_token;

                //没有找到该账户则获取AccessToKen写入存储1200秒
                if (!tokenBLL.ExistsAccount(accountId))
                {
                    tokenBLL.Add(accountId, access_token);
                }
                else
                {
                    //获取公众账户的实体
                    Model.weixin_access_token tokenModel = tokenBLL.GetAccountModel(accountId);
                    //更新到数据库里的AccessToken
                    tokenModel.access_token = access_token;
                    tokenModel.add_time = DateTime.Now;
                    tokenBLL.Update(tokenModel);
                }
            }
            catch (Exception ex)
            {
                error = "获得AccessToken出错:" + ex.Message;
            }
            return access_token;
        }

        /// <summary>
        /// 获得所有关注用户的openid字符串(别的方法调用此方法)
        /// </summary>
        private IList<string> BaseUserOpenId(int uid, out string error)
        {
            IList<string> ret = new List<string>();

            string access_token = GetAccessToken(uid, out error);
            if (error != "")
            {
                return null;
            }
            Senparc.Weixin.MP.AdvancedAPIs.User.OpenIdResultJson openidJson = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Get(access_token, string.Empty);
            if (openidJson.count == openidJson.total)
            {
                ret = openidJson.data.openid;
            }
            else
            {
                GetNextUserOpenId(uid, openidJson.next_openid, ret);
            }
            return ret;
        }

        /// <summary>
        /// (基础方法)获得所有关注用户的openid字符串(递归算法)
        /// </summary>
        private void GetNextUserOpenId(int accountId, string nexOpenid, IList<string> openidList)
        {
            string err = string.Empty;
            string access_token = GetAccessToken(accountId, out err);
            Senparc.Weixin.MP.AdvancedAPIs.User.OpenIdResultJson openidJson = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Get(access_token, nexOpenid);
            if (openidJson == null || openidJson.count <= 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < openidJson.data.openid.Count; i++)
                {
                    openidList.Add(openidJson.data.openid[i]);
                }
                GetNextUserOpenId(accountId, openidJson.next_openid, openidList);
            }
        }

        #region 消息群发处理===================================
        /// <summary>
        /// 上传永久素材
        /// </summary>
        public string UploadForeverMedia(int accountId, string imgFullPath, out string error)
        {
            string accessToken = GetAccessToken(accountId, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return string.Empty;
            }
            var result = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.UploadForeverMedia(accessToken, imgFullPath);
            if (result.errcode == 0)
            {
                return result.media_id;
            }
            error = result.errmsg;
            return string.Empty;
        }

        /// <summary>
        /// 删除永久素材
        /// </summary>
        public bool DeleteForeverMedia(int accountId, string mediaId, out string error)
        {
            string accessToken = GetAccessToken(accountId, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return false;
            }
            var result = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.DeleteForeverMedia(accessToken, mediaId);
            if (result.errcode != 0)
            {
                error = result.errmsg;
                return false;
            }
            error = string.Empty;
            return true;
        }

        /// <summary>
        /// 群发消息
        /// </summary>
        public bool SendGroupMessageByGroupId(int accountId, List<Senparc.Weixin.MP.AdvancedAPIs.GroupMessage.NewsModel> ls, out string error)
        {
            string accessToken = GetAccessToken(accountId, out error);
            //新增素材
            var result1 = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.UploadNews(accessToken, 10000, ls.ToArray());
            if (result1.errcode != 0)
            {
                error = result1.errmsg;
                return false;
            }
            //群发消息
            var result2 = Senparc.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessageByGroupId(accessToken, "0", result1.media_id, Senparc.Weixin.MP.GroupMessageType.mpnews, true);
            if (result2.errcode != 0)
            {
                error = result2.errmsg;
                return false;
            }
            error = string.Empty;
            return true;
        }
        #endregion

    }
}
