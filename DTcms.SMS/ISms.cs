using System;

namespace DTcms.SMS
{
    /// <summary>
    /// 短信接口
    /// </summary>
    public interface ISms
    {
        /// <summary>
        /// 手机短信（单条发送）
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="content">短信内容</param>
        /// <param name="pass">短信通道1验证码通道2广告通道</param>
        /// <param name="msg">返回提示信息</param>
        /// <returns>bool</returns>
        bool Send(string mobile, string content, int pass, out string msg);

        /// <summary>
        /// 手机短信（批量发送）
        /// </summary>
        /// <param name="mobiles">手机号码，以英文“,”逗号分隔开</param>
        /// <param name="content">短信内容</param>
        /// <param name="pass">短信通道1验证码通道2广告通道</param>
        /// <param name="msg">返回提示信息</param>
        /// <returns>bool</returns>
        bool MultiSend(string mobiles, string content, int pass, out string msg);

        /// <summary>
        /// 查询账户剩余短信数量
        /// </summary>
        string GetAccountQuantity();
    }
}
