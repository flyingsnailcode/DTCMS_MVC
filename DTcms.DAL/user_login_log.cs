using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;

namespace DTcms.DAL
{
	/// <summary>
	/// 用户登录日志
	/// </summary>
	public partial class user_login_log : DapperRepository<Model.user_login_log>
    {
        private string databaseprefix; //数据库表名前缀
        public user_login_log(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 同一天内是否有登录过
        /// </summary>
        public bool ExistsDay(string username)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "user_login_log");
            strSql.Append(" where user_name=@0 and DATEDIFF(day,login_time,getdate())=0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), username) > 0;
        }
        #endregion
    }
}

