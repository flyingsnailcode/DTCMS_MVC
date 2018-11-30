using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据库访问层
    /// </summary>
    public partial class sms_log : DapperRepository<Model.sms_log>
    {
        private string databaseprefix; //数据库表名前缀
        public sms_log(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 查询24小时内发送总数
        /// </summary>
        /// <param name="mobile">手机</param>
        /// <returns></returns>
        public int GetSmsCount(string mobile)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) as H from [" + databaseprefix + "sms_log]");
            strSql.Append(" where DateDiff(hh,add_time,getDate())<=24 and mobile=@0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), mobile);
        }
        /// <summary>
        /// 按手机号码查询当于发送次数
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns>总数</returns>
        public int GetMobileCount(string mobile)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) as H from " + databaseprefix + "sms_log");
            strSql.Append(" where datediff(d,send_time,getdate())=0 and mobile=@0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), mobile);
        }
        /// <summary>
        /// 按IP地址查询当天请求短信次数
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>总数</returns>
        public int GetIPCount(string ip)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) as H from " + databaseprefix + "sms_log");
            strSql.Append(" where datediff(d,send_time,getdate())=0 and ip=@0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), ip);
        }
        /// <summary>
        /// 获取当天发送总数
        /// </summary>
        /// <returns>总数</returns>
        public int GetCurDayCount()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) as H from " + databaseprefix + "sms_log");
            strSql.Append(" where datediff(d,send_time,getdate())=0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString());
        }
        #endregion
    }
}
