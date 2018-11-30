using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Linq;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:管理员日志
    /// </summary>
    public partial class manager_log : DapperRepository<Model.manager_log>
    {
        private string databaseprefix; //数据库表名前缀
        public manager_log(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 删除7天前的日志数据
        /// </summary>
        public int Delete(int dayCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "manager_log ");
            strSql.Append(" where DATEDIFF(day, add_time, getdate()) > " + dayCount);
            return WriteDataBase.Delete(strSql.ToString());
        }
        /// <summary>
        /// 返回数据数
        /// </summary>
        public int GetCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) as H from " + databaseprefix + "manager_log");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString());
        }

        /// <summary>
        /// 根据用户名返回上一次登录记录
        /// </summary>
        public Model.manager_log GetModel(string user_name, int top_num, string action_type)
        {
            int rows = GetCount("user_name='" + user_name + "'");
            if (top_num == 1)
            {
                rows = 2;
            }
            if (rows > 1)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select top 1 id from (select top " + top_num + " id from " + databaseprefix + "manager_log");
                strSql.Append(" where user_name=@0 and action_type=@1 order by id desc) as T ");
                strSql.Append(" order by id asc");
                return ReadDataBase.Query<Model.manager_log>(strSql.ToString(), user_name, action_type).FirstOrDefault();
            }
            return null;
        }
        #endregion
    }

}
