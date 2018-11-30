using System;
using System.Collections.Generic;
using System.Data;

namespace DTcms.BLL
{
    /// <summary>
    /// 短信发送日志
    /// </summary>
    public class sms_log : Services<Model.sms_log>
    {
        private DAL.sms_log dal = new DAL.sms_log(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }

        /// <summary>
        /// 按手机号码查询当于发送次数
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns>总数</returns>
        public int GetMobileCount(string mobile)
        {
            return dal.GetMobileCount(mobile);
        }
        /// <summary>
        /// 按IP地址查询当天请求短信次数
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>总数</returns>
        public int GetIPCount(string ip)
        {
            return dal.GetIPCount(ip);
        }
        /// <summary>
        /// 获取当天发送总数
        /// </summary>
        /// <returns>总数</returns>
        public int GetCurDayCount()
        {
            return dal.GetCurDayCount();
        }
    }
}
