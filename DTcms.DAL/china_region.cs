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
    public partial class china_region : DapperRepository<Model.china_region>
    {
        private string databaseprefix; //数据库表名前缀
        public china_region(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 返回标题
        /// </summary>
        /// <param name="id">ID号</param>
        /// <returns>标题</returns>
        public string GetTitle(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select title from " + databaseprefix + "china_region");
            strSql.Append(" where id=@0");
            string salt = ReadDataBase.ExecuteScalar<string>(strSql.ToString(), id);
            if (string.IsNullOrEmpty(salt))
            {
                return string.Empty;
            }
            return salt;
        }
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
            strSql.Append("id,parent_id,title,short_title,class_layer,pinyin,jianpin,fchar,sort_id,is_lock from " + databaseprefix + "china_region");
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
