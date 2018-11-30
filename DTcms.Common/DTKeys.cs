using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Common
{
    public class DTKeys
    {
        //系统版本
        /// <summary>
        /// 版本号全称
        /// </summary>
        public const string ASSEMBLY_VERSION = "4.0.3";
        /// <summary>
        /// 版本年号
        /// </summary>
        public const string ASSEMBLY_YEAR = "2018";
        /// <summary>
        /// 加密配置名称
        /// </summary>
        public const string SYSTEM_OPEN_ENCRYPT = "OpenEncrypt";
        /// <summary>
        /// 安全码
        /// </summary>
        public const string SECURITY_CODE = "2L00BN6H";
        /// <summary>
        /// 加密/解密－密匙
        /// </summary>
        public const string ENCRYPT_KEY = "JTPT6ND0";
        /// <summary>
        /// Cookie 加密密匙
        /// </summary>
        public const string COOKIE_KEY = "D624282L";
        /// <summary>
        /// 用户名称密码缓存
        /// </summary>
        public const string COOKIE_ENCRYPT = "6VZ8XXXF";
        /// <summary>
        /// 升级代码
        /// </summary>
        public const string FILE_URL_UPGRADE_CODE = "267C2643EE401DD2F0A06084F7931C4DEC76E7CAA1996481FE8F5081A8936409058D07A6F5E2941C";
        /// <summary>
        /// 消息代码
        /// </summary>
        public const string FILE_URL_NOTICE_CODE = "267C2643EE401DD2F0A06084F7931C4DEC76E7CAA1996481FE8F5081A8936409D037BEA6A623A0A1";

        //File======================================================
        /// <summary>
        /// 插件配制文件名
        /// </summary>
        public const string FILE_PLUGIN_XML_CONFING = "plugin.config";
        /// <summary>
        /// 站点配置文件名
        /// </summary>
        public const string FILE_SITE_XML_CONFING = "Configpath";
        /// <summary>
        /// URL配置文件名
        /// </summary>
        public const string FILE_URL_XML_CONFING = "Urlspath";
        /// <summary>
        /// 用户配置文件名
        /// </summary>
        public const string FILE_USER_XML_CONFING = "Userpath";
        /// <summary>
        /// 任务调度配置文件名
        /// </summary>
        public const string FILE_JOBS_XML_CONFING = "JobConfig";
        /// <summary>
        /// 订单配置文件名
        /// </summary>
        public const string FILE_ORDER_XML_CONFING = "Orderpath";
        /// <summary>
        /// Lucene.NET配置文件
        /// </summary>
        public const string FILE_LUCENE_XML_CONFING = "LucenePath";

        //Directory==================================================
        /// <summary>
        /// ASPX目录名
        /// </summary>
        public const string DIRECTORY_REWRITE_ASPX = "aspx";
        /// <summary>
        /// MVC目录名
        /// </summary>
        public const string DIRECTORY_REWRITE_MVC = "Areas/Web/Views";
        /// <summary>
        /// HTML目录名
        /// </summary>
        public const string DIRECTORY_REWRITE_HTML = "html";
        /// <summary>
        /// 插件目录名
        /// </summary>
        public const string DIRECTORY_REWRITE_PLUGIN = "plugin";
        /// <summary>
        /// 盘古分词词库路径
        /// </summary>
        public const string FILE_PANGU_DIC_PATH = "DictPath";
        /// <summary>
        /// Lucene.NET索引路径
        /// </summary>
        public const string FILE_LUCENE_DATA_PATH = "IndexesPath";

        //Cache======================================================
        /// <summary>
        /// 站点配置
        /// </summary>
        public const string CACHE_SITE_CONFIG = "dt_cache_site_config";
        /// <summary>
        /// 用户配置
        /// </summary>
        public const string CACHE_USER_CONFIG = "dt_cache_user_config";
        /// <summary>
        /// 订单配置
        /// </summary>
        public const string CACHE_ORDER_CONFIG = "dt_cache_order_config";
        /// <summary>
        /// HttpModule映射类
        /// </summary>
        public const string CACHE_SITE_HTTP_MODULE = "dt_cache_http_module";
        /// <summary>
        /// 绑定域名
        /// </summary>
        public const string CACHE_SITE_HTTP_DOMAIN = "dt_cache_http_domain";
        /// <summary>
        /// 站点一级目录名
        /// </summary>
        public const string CACHE_SITE_DIRECTORY = "dt_cache_site_directory";
        /// <summary>
        /// 站点ASPX目录名
        /// </summary>
        public const string CACHE_SITE_ASPX_DIRECTORY = "dt_cache_site_aspx_directory";
        /// <summary>
        /// URL重写映射表
        /// </summary>
        public const string CACHE_SITE_URLS = "dt_cache_site_urls";
        /// <summary>
        /// 站点所有频道键值对
        /// </summary>
        public const string CACHE_SITE_CHANNEL_LIST = "dt_cache_site_channel_list";
        /// <summary>
        /// URL重写LIST列表
        /// </summary>
        public const string CACHE_SITE_URLS_LIST = "dt_cache_site_urls_list";
        /// <summary>
        /// 升级通知
        /// </summary>
        public const string CACHE_OFFICIAL_UPGRADE = "dt_official_upgrade";
        /// <summary>
        /// 官方消息
        /// </summary>
        public const string CACHE_OFFICIAL_NOTICE = "dt_official_notice";

        //Session=====================================================
        /// <summary>
        /// 网页验证码
        /// </summary>
        public const string SESSION_CODE = "dt_session_code";
        /// <summary>
        /// 短信验证码
        /// </summary>
        public const string SESSION_SMS_CODE = "dt_session_sms_code";
        /// <summary>
        /// 短信手机号码
        /// </summary>
        public const string SESSION_SMS_MOBILE = "dt_session_sms_mobile";
        /// <summary>
        /// 后台管理员
        /// </summary>
        public const string SESSION_ADMIN_INFO = "dt_session_admin_info";
        /// <summary>
        /// 会员用户
        /// </summary>
        public const string SESSION_USER_INFO = "dt_session_user_info";

        //Cookies=====================================================
        /// <summary>
        /// 防重复顶踩KEY
        /// </summary>
        public const string COOKIE_DIGG_KEY = "dt_cookie_digg_key";
        /// <summary>
        /// 防重复评论KEY
        /// </summary>
        public const string COOKIE_COMMENT_KEY = "dt_cookie_comment_key";
        /// <summary>
        /// 记住会员用户名
        /// </summary>
        public const string COOKIE_USER_NAME_REMEMBER = "dt_cookie_user_name_remember";
        /// <summary>
        /// 记住会员密码
        /// </summary>
        public const string COOKIE_USER_PWD_REMEMBER = "dt_cookie_user_pwd_remember";
        /// <summary>
        /// 用户手机号码
        /// </summary>
        public const string COOKIE_USER_MOBILE = "dt_cookie_user_mobile";
        /// <summary>
        /// 用户电子邮箱
        /// </summary>
        public const string COOKIE_USER_EMAIL = "dt_cookie_user_email";
        /// <summary>
        /// 购物车
        /// </summary>
        public const string COOKIE_SHOPPING_CART = "dt_cookie_shopping_cart";
        /// <summary>
        /// 结账清单
        /// </summary>
        public const string COOKIE_SHOPPING_BUY = "dt_cookie_shopping_buy";
        /// <summary>
        /// 返回上一页
        /// </summary>
        public const string COOKIE_URL_REFERRER = "dt_cookie_url_referrer";
        /// <summary>
        /// 搜索内容清单
        /// </summary>
        public const string COOKIE_SEARCH_BUY = "dt_cookie_search_buy";

        //Table=======================================================
        /// <summary>
        /// 频道文章表前缀
        /// </summary>
        public const string TABLE_CHANNEL_ARTICLE = "channel_article_";

        //授权（借鉴小钱袋子）=======================================================
        /// <summary>
        /// 版本
        /// </summary>
        public const string systemVersion = "DTcms.Web for MSSQL V1.0.0";

        public const string tech = "<a href = 'http://myaspx.wang' target='_blank' style='position: fixed; top: 20px; right: 20px; color: #f0f0f0;' title='DTcms.Web for MSSQL V1.0.0'>汪汪博客诚意出品</a>";

        /// <summary>
        /// 总网站地址
        /// </summary>
        public const string systemUrl = "http://myaspx.wang";

        /// <summary>
        /// 序列号  每个网站对应一个   暂时根据IP+安全码+SECURITY_CODE（服务器安全码）生成序列号
        /// </summary>
        public const string systemSN = "CE5EC088910E265AF8F38CB78C9077F9";

        /// <summary>
        /// 安全码（授权）  每个网站对应一个
        /// </summary>
        public const string systemID = "86XD2JVPZ6";

        /// <summary>
        /// 验证URL
        /// </summary>
        public const string checkUrl = "http://myaspx.wang/submit_ajax/sn_myaspx";
        public static bool checkSN(int checkType)
        {
            bool result = false;
            string text = string.Empty;
            if (checkType != 1)
            {
                if (checkType == 2)
                {
                    text = Utils.HttpGet(Utils.CombUrlTxt(checkUrl, "action={0}&sn={1}&idn={2}&ip={3}&sid={4}&timeStamp={5}", new string[]
                    {
                        "login",
                        systemSN,
                        DTRequest.GetHost(),
                        DTRequest.GetServerString("LOCAL_ADDR"),
                        systemID,
                        Utils.GetTimeStamp()
                    }));
                    result = (!(text.ToLower() != "error") || Utils.StrToBool(text, false));
                }
            }
            else
            {
                string linkUrl = Utils.CombUrlTxt(checkUrl, "action={0}&sn={1}&idn={2}&ip={3}&sid={4}&timeStamp={5}", new string[]
                {
                    "install",
                    systemSN,
                    DTRequest.GetHost(),
                    DTRequest.GetServerString("LOCAL_ADDR"),
                    systemID,
                    Utils.GetTimeStamp()
                });
                text = Utils.HttpGet(linkUrl);
                result = (!(text.ToLower() != "error") || Utils.StrToBool(text, false));
            }
            return result;
        }
    }
}
