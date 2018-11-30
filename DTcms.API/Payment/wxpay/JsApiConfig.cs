using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Xml;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.API.Payment.wxpay
{
    public class JsApiConfig
    {
        #region 字段
        private string partner = string.Empty;
        private string key = string.Empty;
        private string appid = string.Empty;
        private string appsecret = string.Empty;
        private string redirect_url = string.Empty;
        private string notify_url = string.Empty;
        #endregion

        public JsApiConfig(int site_payment_id)
        {
            Model.site_payment model = new BLL.site_payment().GetModel(site_payment_id); //站点支付方式
            if (model != null)
            {
                Model.payment payModel = new BLL.payment().GetModel(model.payment_id); //支付平台
                Model.sites siteModel = new BLL.sites().GetModel(model.site_id); //站点配置
                Model.siteconfig sysConfig = new BLL.siteconfig().loadConfig(); //系统配置

                partner = model.key1; //商户号（必须配置）
                key = model.key2; //商户支付密钥，参考开户邮件设置（必须配置）
                appid = model.key3; //绑定支付的APPID（必须配置）
                appsecret = model.key4; //公众帐号secert（仅JSAPI支付的时候需要配置）

                //获取用户的OPENID回调地址及登录后的回调地址
                if (!string.IsNullOrEmpty(siteModel.domain.Trim()) && siteModel.is_default == 0) //如果有自定义域名且不是默认站点
                {
                    redirect_url = "http://" + siteModel.domain + payModel.return_url; //获取用户的OPENID回调地址
                    notify_url = "http://" + siteModel.domain + payModel.notify_url; //登录后的回调地址
                }
                else if (siteModel.is_default == 0) //不是默认站点也没有绑定域名
                {
                    redirect_url = "http://" + HttpContext.Current.Request.Url.Authority.ToLower() + sysConfig.webpath + siteModel.build_path.ToLower() + payModel.return_url;
                    notify_url = "http://" + HttpContext.Current.Request.Url.Authority.ToLower() + sysConfig.webpath + siteModel.build_path.ToLower() + payModel.notify_url;
                }
                else //否则使用当前域名
                {
                    redirect_url = "http://" + HttpContext.Current.Request.Url.Authority.ToLower() + sysConfig.webpath + payModel.return_url;
                    notify_url = "http://" + HttpContext.Current.Request.Url.Authority.ToLower() + sysConfig.webpath + payModel.notify_url;
                }
            }
        }

        #region 属性
        /// <summary>
        /// 商户号（必须配置）
        /// </summary>
        public string Partner
        {
            get { return partner; }
            set { partner = value; }
        }

        /// <summary>
        /// 获取或设交易安全校验码
        /// </summary>
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// 绑定支付的APPID（必须配置）
        /// </summary>
        public string AppId
        {
            get { return appid; }
            set { appid = value; }
        }

        /// <summary>
        /// 公众帐号secert（仅JSAPI支付的时候需要配置）
        /// </summary>
        public string AppSecret
        {
            get { return appsecret; }
            set { appsecret = value; }
        }

        /// <summary>
        /// 获取用户的OPENID回调地址
        /// </summary>
        public string Redirect_url
        {
            get { return redirect_url; }
        }

        /// <summary>
        /// 获取服务器异步通知页面路径
        /// </summary>
        public string Notify_url
        {
            get { return notify_url; }
        }

        #endregion
    }
}
