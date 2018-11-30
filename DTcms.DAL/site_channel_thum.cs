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
    public partial class site_channel_thum : DapperRepository<Model.site_channel_thum>
    {
        private string databaseprefix; //数据库表名前缀
        public site_channel_thum(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        /// <param name="top"></param>
        /// <param name="strWhere">条件</param>
        /// <param name="filedOrder">排序</param>
        /// <returns>DataTable</returns>
        public DataSet GetList(int top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            if (top > 0)
            {
                strSql.AppendFormat("select  top {0} ", top);
            }
            else
            {
                strSql.Append("select ");
            }
            strSql.Append(" * from " + databaseprefix + "site_channel_thum");
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(" where " + strWhere);
            }
            if (!string.IsNullOrEmpty(filedOrder))
            {
                strSql.Append(" order by " + filedOrder);
            }

            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }
        #endregion
    }
}
