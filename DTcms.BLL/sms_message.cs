using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 手机短信
    /// </summary>
    public partial class sms_message : Services<Model.sms_log>
    {
        private DAL.sms_log log = new DAL.sms_log(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = log;
        }
        #region 发送手机短信
        /// <summary>
        /// 发送手机短信
        /// </summary>
        /// <param name="mobiles">手机号码，以英文“,”逗号分隔开</param>
        /// <param name="content">短信内容</param>
        /// <param name="pass">短信通道1验证码通道2广告通道</param>
        /// <param name="msg">返回提示信息</param>
        /// <returns>bool</returns>
        public bool Send(string mobiles, string content, int pass, out string msg)
        {
            //检查是否设置好短信账号
            if (string.IsNullOrEmpty(siteConfig.smsapiurl) || string.IsNullOrEmpty(siteConfig.smsusername) || string.IsNullOrEmpty(siteConfig.smspassword))
            {
                msg = "短信配置参数有误，请完善后再提交！";
                return false;
            }
            //检查成功标识是否设置
            if (string.IsNullOrEmpty(siteConfig.smssendlable))
            {
                msg = "成功标识不能为空，请完善后再提交！";
                return false;
            }
            //检查手机号码，如果超过2000则分批发送
            int sucCount = 0; //成功提交数量
            string errorMsg = string.Empty; //错误消息
            string[] oldMobileArr = mobiles.Split(',');
            int batch = oldMobileArr.Length / 2000 + 1; //2000条为一批，求出分多少批

            //赋值参数
            string temp = string.Empty;
            string result = string.Empty;
            string parameter = siteConfig.smssendpara;
            parameter = parameter.Replace("{uid}", siteConfig.smsusername);
            parameter = parameter.Replace("{pass}", siteConfig.smspassword);
            parameter = parameter.Replace("{md5pass}", Utils.MD5(siteConfig.smspassword));
            if (parameter.Contains("{content}"))
            {
                string _content = content;
                switch (siteConfig.smssign)
                {
                    case 1:
                        _content = siteConfig.smssigntxt + _content;
                        break;
                    case 2:
                        _content += siteConfig.smssigntxt;
                        break;
                }
                parameter = parameter.Replace("{content}", Utils.UrlEncode(_content));
            }
            //加载错误代码
            Dictionary<string, string> dic = ErorrCode();
            //循环发送
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
                    if ("" != RegexHelper.toStr(@"^1\d{10}$", mobile))
                    {
                        if (siteConfig.smssendcount == 0 || log.GetSmsCount(mobile) < siteConfig.smssendcount)
                        {
                            sendCount++;
                            sb.Append(mobile + siteConfig.smsmark);
                        }
                    }
                }
                //发送短信
                if (sb.ToString().Length > 0)
                {
                    try
                    {
                        //号码列表
                        temp = Utils.DelLastChar(sb.ToString(), siteConfig.smsmark);
                        //写入参数
                        parameter = parameter.Replace("{mobile}", temp);
                        //提交方式
                        if ("post" == siteConfig.smssubmit)
                        {
                            if (parameter.StartsWith("?"))
                            {
                                parameter = parameter.Substring(1, parameter.Length - 1);
                            }
                            result = Utils.HttpPost(siteConfig.smsapiurl, parameter);
                        }
                        else
                        {
                            if (!parameter.StartsWith("?"))
                            {
                                parameter = "?" + parameter;
                            }
                            result = Utils.HttpGet(siteConfig.smsapiurl + parameter);
                        }
                        //返回结果类型
                        string txtLable = QueryKey(siteConfig.smssendanswer, siteConfig.smssendcode, result);
                        if (txtLable != siteConfig.smssendlable)
                        {
                            string txtTips = QueryKey(siteConfig.smssendanswer, siteConfig.smssenderror, result);
                            if (dic.Count > 0 && dic.ContainsKey(txtTips))
                            {
                                errorMsg = string.Format("{0}:{1}", txtTips, dic[txtTips]);
                            }
                            else
                            {
                                errorMsg = txtTips;
                            }
                            //记录日志
                            LogHelper.Error(string.Format("{0}：{1}", QueryMobile(sb.ToString()), errorMsg));
                            continue;
                        }
                        sucCount += sendCount; //成功数量
                        //写入短信记录
                        string[] arr = temp.Replace(siteConfig.smsmark, ",").Split(',');
                        foreach (string m in arr)
                        {
                            log.Add(new Model.sms_log { mobile = m, content = content, send_time = DateTime.Now });
                        }
                    }
                    catch
                    {
                        //没有动作
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
        #endregion

        #region 查询账户剩余短信数量
        /// <summary>
        /// 查询账户剩余短信数量
        /// </summary>
        public bool GetAccountQuantity(out int total, out string errorMsg)
        {
            //检查是否设置好短信账号
            if (string.IsNullOrEmpty(siteConfig.smsqueryapiurl) || string.IsNullOrEmpty(siteConfig.smsusername) || string.IsNullOrEmpty(siteConfig.smspassword))
            {
                total = 0;
                errorMsg = "查询出错：请完善账户信息";
                return false;
            }
            try
            {
                string result;
                string parameter = siteConfig.smsquerypara;
                parameter = parameter.Replace("{uid}", siteConfig.smsusername);
                parameter = parameter.Replace("{pass}", siteConfig.smspassword);
                parameter = parameter.Replace("{md5pass}", Utils.MD5(siteConfig.smspassword));
                //提交方式
                if ("post" == siteConfig.smssubmit)
                {
                    if (parameter.StartsWith("?"))
                    {
                        parameter = parameter.Substring(1, parameter.Length - 1);
                    }
                    result = Utils.HttpPost(siteConfig.smsapiurl, parameter);
                }
                else
                {
                    if (!parameter.StartsWith("?"))
                    {
                        parameter = "?" + parameter;
                    }
                    result = Utils.HttpGet(siteConfig.smsapiurl + parameter);
                }
                if (string.IsNullOrEmpty(siteConfig.smsquerycode))
                {
                    //返回结果
                    if (string.IsNullOrEmpty(siteConfig.smsqueryresult))
                    {
                        total = Utils.StrToInt(result, 0);
                    }
                    else
                    {
                        total = Utils.StrToInt(QueryKey(siteConfig.smsqueryanswer, siteConfig.smsqueryresult, result), 0);
                    }
                    //返回结果
                    errorMsg = string.Empty;
                    return true;
                }
                else
                {
                    //查询成功标识
                    string txtLable = QueryKey(siteConfig.smsqueryanswer, siteConfig.smsquerycode, result);
                    if (txtLable != siteConfig.smsquerylable)
                    {
                        total = 0;
                        string txtTips = QueryKey(siteConfig.smsqueryanswer, siteConfig.smsqueryerror, result);
                        //加载错误代码
                        Dictionary<string, string> dic = ErorrCode();
                        if (dic.Count > 0 && dic.ContainsKey(txtTips))
                        {
                            errorMsg = string.Format("{0}:{1}", txtTips, dic[txtTips]);
                        }
                        else
                        {
                            errorMsg = txtTips;
                        }
                        return false;
                    }
                    else
                    {
                        total = Utils.StrToInt(QueryKey(siteConfig.smsqueryanswer, siteConfig.smsqueryresult, result), 0);
                        errorMsg = string.Empty;
                        return true;
                    }
                }
            }
            catch
            {
                total = 0;
                errorMsg = "发生未知错误";
                return false;
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 手机号码中间4位用*代替
        /// </summary>
        private string QueryMobile(string mobile)
        {
            return mobile.Insert(3, "****").Remove(7, 4);
        }
        /// <summary>
        /// 查询结果
        /// </summary>
        /// <param name="i">类型</param>
        /// <param name="code">KEY</param>
        /// <param name="result">文本</param>
        /// <returns></returns>
        private string QueryKey(int i, string key, string result)
        {
            if (key.Length > 0 && i >= 0 && i <= 2)
            {
                if (i > 1)
                {
                    return XmlHelper.GetNodesValue(result, key);
                }
                else if (i > 0)
                {
                    return JsonHelper.GetString(result, key);
                }
                else
                {
                    return RegexHelper.toStr(key, result);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 错误代码
        /// </summary>
        private Dictionary<string, string> ErorrCode()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] valArr = siteConfig.smserrorcode.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < valArr.Length; i++)
            {
                string[] valItemArr = valArr[i].Split('|');
                if (valItemArr.Length == 2)
                {
                    dic.Add(valItemArr[0], valItemArr[1]);
                }
            }
            return dic;
        }
        #endregion
    }
}