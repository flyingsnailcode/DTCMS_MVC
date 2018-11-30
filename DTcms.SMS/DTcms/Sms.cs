using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DTcms.Common;

namespace DTcms.SMS
{
    /// <summary>
    /// 动力启航软
    /// DTcms 系统默认自带
    /// 2017-10-18
    /// http://www.dtcms.net/
    /// </summary>
    public class Sms : ISms
    {
        /// <summary>
        /// 账号
        /// </summary>
        private readonly string username;
        /// <summary>
        /// 登录密码
        /// </summary>
        private readonly string password;
        /// <summary>
        /// API地址
        /// </summary>
        private const string apiurl = "http://smsapi.dtcms.net/httpapi/";
        /// <summary>
        /// 同一手机单日最大允许发送量
        /// </summary>
        private readonly int sendCount;
        /// <summary>
        /// 同一IP地址，单日最大允许提交请求次数
        /// </summary>
        private readonly int ipCount;
        /// <summary>
        /// 平台单日发送上限
        /// </summary>
        private readonly int safeTotal;
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="username">账号</param>
        /// <param name="password">登录密码</param>
        /// <param name="sendCount">同一手机单日最大允许发送量</param>
        /// <param name="ipCount">同一IP地址，单日最大允许提交请求次数</param>
        /// <param name="safeTotal">平台单日发送上限</param>
        public Sms(string uid, string password, int sendCount, int ipCount, int safeTotal)
        {
            this.username = uid;
            if (!string.IsNullOrEmpty(password))
            {
                this.password = Utils.MD5(DESEncrypt.Decrypt(password));
            }
            else
            {
                this.password = string.Empty;
            }
            this.sendCount = sendCount;
            this.ipCount = ipCount;
            this.safeTotal = safeTotal;
        }

        /// <summary>
        /// 手机短信（单条发送）
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="content">短信内容</param>
        /// <param name="pass">短信通道1验证码通道2广告通道</param>
        /// <param name="msg">返回提示信息</param>
        /// <returns>bool</returns>
        public bool Send(string mobile, string content, int pass, out string msg)
        {
            if (string.IsNullOrEmpty(this.username) || string.IsNullOrEmpty(this.password))
            {
                msg = "短信配置参数有误，请完善后再提交！";
                return false;
            }
            //验证手机号码
            Regex r = new Regex(@"^1(3|4|5|7|8)\d{9}$", RegexOptions.IgnoreCase);
            if (!r.Match(mobile).Success)
            {
                msg = "手机号码格式不正确！";
                return false;
            }

            bool status = true;
            BLL.sms_log bll = new BLL.sms_log();
            //查询是否超出平台限制
            int thisSafeTotal = bll.GetCurDayCount();
            if (this.safeTotal > 0 && thisSafeTotal > this.safeTotal)
            {
                msg = "对不起，平台短信发送量已超出最大限制！";
                status = false;
            }

            //查询当前IP是否已经超出限制
            string ip = DTRequest.GetIP();
            int thisIpSendCount = bll.GetIPCount(ip);
            if (this.ipCount > 0 && thisIpSendCount > this.ipCount)
            {
                msg = "对不起，你的网络已经超出发送数量限制！";
                status = false;
            }
            msg = string.Empty;
            if (status)
            {
                //发送短信
                Model.sms_log model = new Model.sms_log();
                model.mobile = mobile;
                model.content = content;
                model.send_time = DateTime.Now;
                try
                {
                    string result = Utils.HttpPost(apiurl,
                        "cmd=tx&pass=" + pass + "&uid=" + this.username + "&pwd=" + this.password + "&mobile=" + mobile + "&encode=utf8&content=" + Utils.UrlEncode(content));
                    string[] strArr = result.Split(new string[] { "||" }, StringSplitOptions.None);
                    if (strArr[0] != "100")
                    {
                        status = false;
                        model.status = 1;
                        model.remark = "提交失败，错误提示：" + strArr[1];
                        msg = model.remark;
                    }
                    else
                    {
                        model.status = 0;
                        model.remark = strArr[0];
                        msg = model.remark;
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    model.status = 1;
                    model.remark = "提交失败，错误提示：" + ex.Message;
                    msg = model.remark;
                }
                bll.Add(model);
            }
            //返回状态
            if (status)
            {
                msg = "发送成功!";
                return true;
            }
            return false;
        }

        /// <summary>
        /// 手机短信（批量发送）
        /// 注意：批量发送只验证平台发送总数限制；
        /// </summary>
        /// <param name="mobiles">手机号码，以英文“,”逗号分隔开</param>
        /// <param name="content">短信内容</param>
        /// <param name="pass">短信通道1验证码通道2广告通道</param>
        /// <param name="msg">返回提示信息</param>
        /// <returns>bool</returns>
        public bool MultiSend(string mobiles, string content, int pass, out string msg)
        {
            msg = "";
            if (string.IsNullOrEmpty(this.username) || string.IsNullOrEmpty(this.password))
            {
                msg = "短信配置参数有误，请完善后再提交！";
                return false;
            }
            int sucCount = 0;
            string[] oldMobileArr = mobiles.Split(',');
            
            //查询是否超出平台限制
            int thisSafeTotal = new BLL.sms_log().GetCurDayCount();
            if (this.safeTotal > 0 && (thisSafeTotal > this.safeTotal || thisSafeTotal + oldMobileArr.Length > this.safeTotal))
            {
                msg = "对不起，平台短信发送量已超出最大限制！";
                return false;
            }
            //错误消息
            string errorMsg = string.Empty;
            //验证手机号码
            Regex r = new Regex(@"^1(3|4|5|7|8)\d{9}$", RegexOptions.IgnoreCase);
            //2000条为一批，求出分多少批
            int batch = oldMobileArr.Length / 2000 + 1;
            for (int i = 0; i < batch; i++)
            {
                StringBuilder sb = new StringBuilder();
                int sendCount = 0; //发送数量
                int maxLenght = (i + 1) * 2000; //循环最大的数

                //检测号码，忽略不合格的，重新组合
                for (int j = 0; j < oldMobileArr.Length && j < maxLenght; j++)
                {
                    int arrNum = j + (i * 2000);
                    string mobile = oldMobileArr[arrNum].Trim();
                    Match m = r.Match(mobile); //搜索匹配项
                    if (m != null)
                    {
                        sendCount++;
                        sb.Append(mobile + ",");
                    }
                }

                //发送短信
                if (sb.ToString().Length > 0)
                {
                    try
                    {
                        string result = Utils.HttpPost(apiurl,
                            "cmd=tx&pass=" + pass + "&uid=" + this.username + "&pwd=" + this.password + "&mobile=" + Utils.DelLastComma(sb.ToString()) + "&encode=utf8&content=" + Utils.UrlEncode(content));
                        string[] strArr = result.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (strArr[0] != "100")
                        {
                            errorMsg = "提交失败，错误提示：" + strArr[1];
                            continue;
                        }
                        sucCount += sendCount; //成功数量
                    }
                    catch (Exception ex)
                    {
                        errorMsg = "提交失败，错误提示：" + ex.Message;
                    }
                }
            }
            //返回状态
            if (sucCount > 0)
            {
                msg = "成功提交" + sucCount + "条，失败" + (oldMobileArr.Length - sucCount) + "条";
                return true;
            }
            msg = errorMsg;
            return false;
        }

        /// <summary>
        /// 查询账户剩余短信数量
        /// </summary>
        public string GetAccountQuantity()
        {
            StringBuilder txt = new StringBuilder();
            //检查是否设置好短信账号
            if (string.IsNullOrEmpty(this.username) || string.IsNullOrEmpty(this.password))
            {
                txt.Append("<span>短信配置参数有误，请完善后再提交！</span>");
            }
            else
            {
                try
                {
                    string result = Utils.HttpPost(apiurl, "cmd=mm&uid=" + this.username + "&pwd=" + this.password);
                    string[] strArr = result.Split(new string[] { "||" }, StringSplitOptions.None);
                    if (strArr[0] != "100")
                    {
                        txt.Append("<span>错误代码：" + strArr[0] + "</span>");
                    }
                    else
                    {
                        txt.Append("<span>" + strArr[1] + " 条</span>");
                    }
                }
                catch
                {
                    txt.Append("<span>查询出错：请完善账户信息！</span>");
                }
            }
            txt.Append(@"<span class=""Validform_checktip"">尚未申请？<a href=""http://www.dtcms.net"" target=""_blank"">请点击这里注册</a></span>");
            return txt.ToString();
        }

        /// <summary>
        /// 错误列表
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> ErrorDic()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("0", "发送短信成功");
            dic.Add("1", "提交参数不能为空");
            dic.Add("2", "账号无效或权限不足");
            dic.Add("3", "账号密码错误");
            dic.Add("4", "预约发送时间格式不正确，应为yyyyMMddHHmmss");
            dic.Add("5", "IP不合法");
            dic.Add("6", "号码中含有无效号码或不在规定的号段或为免打扰号码（含系统黑名单号码）");
            dic.Add("7", "非法关键字");
            dic.Add("8", "内容长度超过上限，最大402字或字符");
            dic.Add("9", "接受号码过多，最大1000");
            dic.Add("11", "提交速度太快");
            dic.Add("12", "您尚未订购[普通短信业务]，暂不能发送该类信息");
            dic.Add("13", "您的[普通短信业务]剩余数量发送不足，暂不能发送该类信息");
            dic.Add("14", "流水号格式不正确");
            dic.Add("15", "流水号重复");
            dic.Add("16", "超出发送上限（操作员帐户当日发送上限）");
            dic.Add("17", "余额不足");
            dic.Add("18", "扣费不成功");
            dic.Add("20", "系统错误");
            dic.Add("21", "密码错误次数达到5次");
            dic.Add("24", "帐户状态不正常");
            dic.Add("25", "账户权限不足");
            dic.Add("26", "需要人工审核");
            dic.Add("28", "发送内容与模板不符");
            dic.Add("29", "扩展号太长或不是数字&accnum=");
            dic.Add("32", "同一号码相同内容发送次数太多（默认24小时内，验证码类发送6次或相同内容3次以上会报此错误。）");
            return dic;
        }
    }
}
