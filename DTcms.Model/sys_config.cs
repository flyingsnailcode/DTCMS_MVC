using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Model
{
    /// <summary>
    /// 站点配置实体类
    /// </summary>
    [Serializable]
    public class siteconfig
    {
        public siteconfig()
        { }

        private string _webname = "";
        private string _weburl = "";
        private string _webcompany = "";
        private string _webaddress = "";
        private string _webtel = "";
        private string _webfax = "";
        private string _webmail = "";
        private string _webcrod = "";

        private string _webpath = "";
        private string _webmanagepath = "";
        private int _staticstatus = 0;
        private string _staticextension = "";
        private int _memberstatus = 1;
        private int _commentstatus = 0;
        private int _commentsemail = 0;
        private int _logstatus = 0;
        private int _webstatus = 1;
        private string _webclosereason = "";
        private string _webcountcode = "";
        
        //2015-09-24  通用API接口改造
        private string _smssubmit = "";
        private string _smsapikey = "";
        private string _smssendpara = "";
        private int _smssign = 0;
        private string _smssigntxt = "";
        private int _smssendanswer = 0;
        private string _smsmark = "";
        private string _smssendcode = "";
        private string _smssenderror = "";
        private string _smssendlable = "";
        private string _smsqueryapiurl = "";
        private string _smsquerypara = "";
        private int _smsqueryanswer = 0;
        private string _smsquerycode = "";
        private string _smsqueryresult = "";
        private string _smsqueryerror = "";
        private string _smsquerylable = "";
        private string _smsqueryformat = "";
        private string _smserrorcode = "";

        private string _smsplatform = string.Empty;
        private string _smsapiurl = "dt";
        private string _smsusername = string.Empty;
        private string _smspassword = string.Empty;
        private string _smsapicode = string.Empty;
        private int _smssendcount = 10;
        private int _smsipcount = 20;
        private int _smssafecount = 1000;

        private string _emailsmtp = "";
        private int _emailssl = 0;
        private int _emailport = 25;
        private string _emailfrom = "";
        private string _emailusername = "";
        private string _emailpassword = "";
        private string _emailnickname = "";

        private int _fileservice = 0;
        private string _filepath = "";
        private string _pathdomain = "";
        private string _localpath = "";
        private int _filesave = 1;
        private int _fileremote = 0;
        private string _imgextension = "";
        private string _attachextension = "";
        private string _videoextension = "";
        private int _attachsize = 0;
        private int _videosize = 0;
        private int _imgsize = 0;
        private int _imgmaxheight = 0;
        private int _imgmaxwidth = 0;
        private int _thumbnailheight = 0;
        private int _thumbnailwidth = 0;
        private int _imgplugin = 0;  //图片处理插件 0 默认、1 ImageMagick
        private string _imgconvert = string.Empty;   //图版格式自动转换
        private string _thumbnailmode = "Cut";
        private int _watermarktype = 0;
        private int _watermarkposition = 9;
        private int _watermarkimgquality = 80;
        private string _watermarkpic = "";
        private int _watermarktransparency = 10;
        private string _watermarktext = "";
        private string _watermarkfont = "";
        private int _watermarkfontsize = 12;

        private int _systemcache = 0;
        private int _cachetime = 3;
        private string _cachekey = "CacheKey";
        private int _deltable = 0;
        private string _author = "";
        private string _source = "";
        private string _rewardalipay = "";
        private string _rewardwx = "";
        private string _followwx = "";

        private string _sysdatabaseprefix = "dt_";
        private string _sysencryptstring = "DTcms";

        
        private string _fileserver = "localhost";
        private string _domainpath = string.Empty;
        private string _domainbind = string.Empty;
        private string _endpoint = "";
        private string _accessid = "";
        private string _accesssecret = "";
        private string _spacename = "";
        private string _spaceurl = "";
        
        #region 主站基本信息==================================
        /// <summary>
        /// 网站名称
        /// </summary>
        public string webname
        {
            get { return _webname; }
            set { _webname = value; }
        }
        /// <summary>
        /// 网站域名
        /// </summary>
        public string weburl
        {
            get { return _weburl; }
            set { _weburl = value; }
        }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string webcompany
        {
            get { return _webcompany; }
            set { _webcompany = value; }
        }
        /// <summary>
        /// 通讯地址
        /// </summary>
        public string webaddress
        {
            get { return _webaddress; }
            set { _webaddress = value; }
        }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string webtel
        {
            get { return _webtel; }
            set { _webtel = value; }
        }
        /// <summary>
        /// 传真号码
        /// </summary>
        public string webfax
        {
            get { return _webfax; }
            set { _webfax = value; }
        }
        /// <summary>
        /// 管理员邮箱
        /// </summary>
        public string webmail
        {
            get { return _webmail; }
            set { _webmail = value; }
        }
        /// <summary>
        /// 网站备案号
        /// </summary>
        public string webcrod
        {
            get { return _webcrod; }
            set { _webcrod = value; }
        }
        #endregion

        #region 功能权限设置==================================
        /// <summary>
        /// 网站安装目录
        /// </summary>
        public string webpath
        {
            get { return _webpath; }
            set { _webpath = value; }
        }
        /// <summary>
        /// 网站管理目录
        /// </summary>
        public string webmanagepath
        {
            get { return _webmanagepath; }
            set { _webmanagepath = value; }
        }
        /// <summary>
        /// 是否开启生成静态
        /// </summary>
        public int staticstatus
        {
            get { return _staticstatus; }
            set { _staticstatus = value; }
        }
        /// <summary>
        /// 生成静态扩展名
        /// </summary>
        public string staticextension
        {
            get { return _staticextension; }
            set { _staticextension = value; }
        }
        /// <summary>
        /// 开启会员功能
        /// </summary>
        public int memberstatus
        {
            get { return _memberstatus; }
            set { _memberstatus = value; }
        }
        /// <summary>
        /// 开启评论通知
        /// </summary>
        public int commentstatus
        {
            get { return _commentsemail; }
            set { _commentsemail = value; }
        }
        /// <summary>
        /// 开启评论审核
        /// </summary>
        public int commentsemail
        {
            get { return _commentstatus; }
            set { _commentstatus = value; }
        }
        /// <summary>
        /// 后台管理日志
        /// </summary>
        public int logstatus
        {
            get { return _logstatus; }
            set { _logstatus = value; }
        }
        /// <summary>
        /// 是否关闭网站
        /// </summary>
        public int webstatus
        {
            get { return _webstatus; }
            set { _webstatus = value; }
        }
        /// <summary>
        /// 关闭原因描述
        /// </summary>
        public string webclosereason
        {
            get { return _webclosereason; }
            set { _webclosereason = value; }
        }
        /// <summary>
        /// 网站统计代码
        /// </summary>
        public string webcountcode
        {
            get { return _webcountcode; }
            set { _webcountcode = value; }
        }
        #endregion

        #region 短信平台设置==================================
        /// <summary>
        /// 短信平台
        /// </summary>
        public string smsplatform
        {
            get { return _smsplatform; }
            set { _smsplatform = value; }
        }
        /// <summary>
        /// 企业编号
        /// </summary>
        public string smsapicode
        {
            get { return _smsapicode; }
            set { _smsapicode = value; }
        }
        /// <summary>
        /// 同一IP单日允许发送总数
        /// </summary>
        public int smsipcount
        {
            get { return _smsipcount; }
            set { _smsipcount = value; }
        }
        /// <summary>
        /// 平台单日最大允许发送总数
        /// </summary>
        public int smssafecount
        {
            get { return _smssafecount; }
            set { _smssafecount = value; }
        }
        /// <summary>
        /// 短信API地址
        /// </summary>
        public string smsapiurl
        {
            get { return _smsapiurl; }
            set { _smsapiurl = value; }
        }
        /// <summary>
        /// 短信平台登录账户名
        /// </summary>
        public string smsusername
        {
            get { return _smsusername; }
            set { _smsusername = value; }
        }
        /// <summary>
        /// 短信平台登录密码
        /// </summary>
        public string smspassword
        {
            get { return _smspassword; }
            set { _smspassword = value; }
        }
        /// <summary>
        /// 提交方式
        /// </summary>
        public string smssubmit
        {
            set { _smssubmit = value; }
            get { return _smssubmit; }
        }
        /// <summary>
        /// APIKEY密钥
        /// </summary>
        public string smsapikey
        {
            set { _smsapikey = value; }
            get { return _smsapikey; }
        }
        /// <summary>
        /// 发送接口参数
        /// </summary>
        public string smssendpara
        {
            set { _smssendpara = value; }
            get { return _smssendpara; }
        }
        /// <summary>
        /// 是否加入签名
        /// </summary>
        public int smssign
        {
            set { _smssign = value; }
            get { return _smssign; }
        }
        /// <summary>
        /// 签名名称
        /// </summary>
        public string smssigntxt
        {
            set { _smssigntxt = value; }
            get { return _smssigntxt; }
        }
        /// <summary>
        /// 发送响应代码
        /// </summary>
        public int smssendanswer
        {
            set { _smssendanswer = value; }
            get { return _smssendanswer; }
        }
        /// <summary>
        /// 响应状态查询
        /// </summary>
        public string smssendcode
        {
            set { _smssendcode = value; }
            get { return _smssendcode; }
        }
        /// <summary>
        /// 响应错误查询
        /// </summary>
        public string smssenderror
        {
            set { _smssenderror = value; }
            get { return _smssenderror; }
        }
        /// <summary>
        /// 发送响应成功标识
        /// </summary>
        public string smssendlable
        {
            set { _smssendlable = value; }
            get { return _smssendlable; }
        }
        /// <summary>
        /// 24小时最大发送总数
        /// </summary>
        public int smssendcount
        {
            set { _smssendcount = value; }
            get { return _smssendcount; }
        }
        /// <summary>
        /// 批量短信间隔符号
        /// </summary>
        public string smsmark
        {
            set { _smsmark = value; }
            get { return _smsmark; }
        }
        /// <summary>
        /// 查询API接口
        /// </summary>
        public string smsqueryapiurl
        {
            set { _smsqueryapiurl = value; }
            get { return _smsqueryapiurl; }
        }
        /// <summary>
        /// 查询接口参数
        /// </summary>
        public string smsquerypara
        {
            set { _smsquerypara = value; }
            get { return _smsquerypara; }
        }
        /// <summary>
        /// 响应结果类型
        /// </summary>
        public int smsqueryanswer
        {
            set { _smsqueryanswer = value; }
            get { return _smsqueryanswer; }
        }
        /// <summary>
        /// 响应状态查询
        /// </summary>
        public string smsquerycode
        {
            set { _smsquerycode = value; }
            get { return _smsquerycode; }
        }
        /// <summary>
        /// 响应结果查询
        /// </summary>
        public string smsqueryresult
        {
            set { _smsqueryresult = value; }
            get { return _smsqueryresult; }
        }
        /// <summary>
        /// 响应错误查询
        /// </summary>
        public string smsqueryerror
        {
            set { _smsqueryerror = value; }
            get { return _smsqueryerror; }
        }
        /// <summary>
        /// 响应成功标识
        /// </summary>
        public string smsquerylable
        {
            set { _smsquerylable = value; }
            get { return _smsquerylable; }
        }
        /// <summary>
        /// 显示结果格式
        /// </summary>
        public string smsqueryformat
        {
            set { _smsqueryformat = value; }
            get { return _smsqueryformat; }
        }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string smserrorcode
        {
            set { _smserrorcode = value; }
            get { return _smserrorcode; }
        }
        #endregion

        #region 邮件发送设置==================================
        /// <summary>
        /// STMP服务器
        /// </summary>
        public string emailsmtp
        {
            get { return _emailsmtp; }
            set { _emailsmtp = value; }
        }
        /// <summary>
        /// 是否启用SSL加密连接
        /// </summary>
        public int emailssl
        {
            get { return _emailssl; }
            set { _emailssl = value; }
        }
        /// <summary>
        /// SMTP端口
        /// </summary>
        public int emailport
        {
            get { return _emailport; }
            set { _emailport = value; }
        }
        /// <summary>
        /// 发件人地址
        /// </summary>
        public string emailfrom
        {
            get { return _emailfrom; }
            set { _emailfrom = value; }
        }
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string emailusername
        {
            get { return _emailusername; }
            set { _emailusername = value; }
        }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string emailpassword
        {
            get { return _emailpassword; }
            set { _emailpassword = value; }
        }
        /// <summary>
        /// 发件人昵称
        /// </summary>
        public string emailnickname
        {
            get { return _emailnickname; }
            set { _emailnickname = value; }
        }
        #endregion

        #region 文件上传设置==================================
        /// <summary>
        /// 存储服务器
        /// </summary>
        public int fileservice
        {
            get { return _fileservice; }
            set { _fileservice = value; }
        }
        /// <summary>
        /// 附件上传目录
        /// </summary>
        public string filepath
        {
            get { return _filepath; }
            set { _filepath = value; }
        }
        /// <summary>
        /// 本地路径绑定域名
        /// </summary>
        public string pathdomain
        {
            get { return _pathdomain; }
            set { _pathdomain = value; }
        }
        /// <summary>
        /// 本地路径
        /// </summary>
        public string localpath
        {
            get { return _localpath; }
            set { _localpath = value; }
        }
        /// <summary>
        /// 附件保存方式
        /// </summary>
        public int filesave
        {
            get { return _filesave; }
            set { _filesave = value; }
        }
        /// <summary>
        /// 编辑器远程图片上传
        /// </summary>
        public int fileremote
        {
            get { return _fileremote; }
            set { _fileremote = value; }
        }
        /// <summary>
        /// 图片上传类型
        /// </summary>
        public string imgextension
        {
            get { return _imgextension; }
            set { _imgextension = value; }
        }
        /// <summary>
        /// 附件上传类型
        /// </summary>
        public string attachextension
        {
            get { return _attachextension; }
            set { _attachextension = value; }
        }
        /// <summary>
        /// 视频上传类型
        /// </summary>
        public string videoextension
        {
            get { return _videoextension; }
            set { _videoextension = value; }
        }
        /// <summary>
        /// 文件上传大小
        /// </summary>
        public int attachsize
        {
            get { return _attachsize; }
            set { _attachsize = value; }
        }
        /// <summary>
        /// 视频上传大小
        /// </summary>
        public int videosize
        {
            get { return _videosize; }
            set { _videosize = value; }
        }
        /// <summary>
        /// 图片上传大小
        /// </summary>
        public int imgsize
        {
            get { return _imgsize; }
            set { _imgsize = value; }
        }
        /// <summary>
        /// 图片最大高度(像素)
        /// </summary>
        public int imgmaxheight
        {
            get { return _imgmaxheight; }
            set { _imgmaxheight = value; }
        }
        /// <summary>
        /// 图片最大宽度(像素)
        /// </summary>
        public int imgmaxwidth
        {
            get { return _imgmaxwidth; }
            set { _imgmaxwidth = value; }
        }
        /// <summary>
        /// 生成缩略图高度(像素)
        /// </summary>
        public int thumbnailheight
        {
            get { return _thumbnailheight; }
            set { _thumbnailheight = value; }
        }
        /// <summary>
        /// 生成缩略图宽度(像素)
        /// </summary>
        public int thumbnailwidth
        {
            get { return _thumbnailwidth; }
            set { _thumbnailwidth = value; }
        }
        /// <summary>
        /// 图片处理插件 0 默认、1 ImageMagick
        /// </summary>
        public int imgplugin
        {
            get { return _imgplugin; }
            set { _imgplugin = value; }
        }
        /// <summary>
        /// 缩略图生成方式
        /// </summary>
        public string thumbnailmode
        {
            get { return _thumbnailmode; }
            set { _thumbnailmode = value; }
        }
        /// <summary>
        /// 图片水印类型
        /// </summary>
        public int watermarktype
        {
            get { return _watermarktype; }
            set { _watermarktype = value; }
        }
        /// <summary>
        /// 图片水印位置
        /// </summary>
        public int watermarkposition
        {
            get { return _watermarkposition; }
            set { _watermarkposition = value; }
        }
        /// <summary>
        /// 图片生成质量
        /// </summary>
        public int watermarkimgquality
        {
            get { return _watermarkimgquality; }
            set { _watermarkimgquality = value; }
        }
        /// <summary>
        /// 图片水印文件
        /// </summary>
        public string watermarkpic
        {
            get { return _watermarkpic; }
            set { _watermarkpic = value; }
        }
        /// <summary>
        /// 水印透明度
        /// </summary>
        public int watermarktransparency
        {
            get { return _watermarktransparency; }
            set { _watermarktransparency = value; }
        }
        /// <summary>
        /// 水印文字
        /// </summary>
        public string watermarktext
        {
            get { return _watermarktext; }
            set { _watermarktext = value; }
        }
        /// <summary>
        /// 文字字体
        /// </summary>
        public string watermarkfont
        {
            get { return _watermarkfont; }
            set { _watermarkfont = value; }
        }
        /// <summary>
        /// 文字大小(像素)
        /// </summary>
        public int watermarkfontsize
        {
            get { return _watermarkfontsize; }
            set { _watermarkfontsize = value; }
        }
        #endregion

        #region 其它扩展设置================================
        /// <summary>
        /// 系统缓存
        /// </summary>
        public int systemcache
        {
            get { return _systemcache; }
            set { _systemcache = value; }
        }
        /// <summary>
        /// 缓存时间
        /// </summary>
        public int cachetime
        {
            get { return _cachetime; }
            set { _cachetime = value; }
        }
        /// <summary>
        /// 缓存标识
        /// </summary>
        public string cachekey
        {
            get { return _cachekey; }
            set { _cachekey = value; }
        }
        /// <summary>
        /// 卸载插件时是否删除插件表
        /// </summary>
        public int deltable
        {
            get { return _deltable; }
            set { _deltable = value; }
        }
        /// <summary>
        /// 作者
        /// </summary>
        public string author
        {
            get { return _author; }
            set { _author = value; }
        }
        /// <summary>
        /// 来源
        /// </summary>
        public string source
        {
            get { return _source; }
            set { _source = value; }
        }
        /// <summary>
        /// 微信关注
        /// </summary>
        public string followwx
        {
            get { return _followwx; }
            set { _followwx = value; }
        }

        /// <summary>
        /// 支付宝打赏
        /// </summary>
        public string rewardalipay
        {
            get { return _rewardalipay; }
            set { _rewardalipay = value; }
        }

        /// <summary>
        /// 微信打赏
        /// </summary>
        public string rewardwx
        {
            get { return _rewardwx; }
            set { _rewardwx = value; }
        }
        #endregion

        #region 安装初始化设置================================
        /// <summary>
        /// 数据库表前缀
        /// </summary>
        public string sysdatabaseprefix
        {
            get { return _sysdatabaseprefix; }
            set { _sysdatabaseprefix = value; }
        }
        /// <summary>
        /// 加密字符串
        /// </summary>
        public string sysencryptstring
        {
            get { return _sysencryptstring; }
            set { _sysencryptstring = value; }
        }
        #endregion

        #region 文件服务器==================================
        /// <summary>
        /// 图版格式自动转换 支持jpg、png...
        /// </summary>
        public string imgconvert
        {
            get { return _imgconvert; }
            set { _imgconvert = value; }
        }
        /// <summary>
        /// 文件服务器(本地localhost、阿里云aliyun、腾讯云qcloud)
        /// </summary>
        public string fileserver
        {
            get { return _fileserver; }
            set { _fileserver = value; }
        }
        /// <summary>
        /// 跨域保存路径
        /// </summary>
        public string domainpath
        {
            get { return _domainpath; }
            set { _domainpath = value; }
        }
        /// <summary>
        /// 跨域绑定域名
        /// </summary>
        public string domainbind
        {
            get { return _domainbind; }
            set { _domainbind = value; }
        }
        /// <summary>
        /// 外网Endpoint
        /// </summary>
        public string endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }
        /// <summary>
        /// Access Key ID
        /// </summary>
        public string accessid
        {
            get { return _accessid; }
            set { _accessid = value; }
        }
        /// <summary>
        /// Access Key Secret
        /// </summary>
        public string accesssecret
        {
            get { return _accesssecret; }
            set { _accesssecret = value; }
        }
        /// <summary>
        /// 图片上传--空间名
        /// </summary>
        public string spacename
        {
            get { return _spacename; }
            set { _spacename = value; }
        }
        /// <summary>
        /// 返回的图片的src
        /// </summary>
        public string spaceurl
        {
            get { return _spaceurl; }
            set { _spaceurl = value; }
        }
        #endregion
    }
}
