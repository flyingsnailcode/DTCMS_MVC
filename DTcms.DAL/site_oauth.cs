using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using DTcms.DBUtility;
using DTcms.Common;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:所属站点的OAuth登录表
    /// </summary>
    public partial class site_oauth : DapperRepository<Model.site_oauth>
    {
        private string databaseprefix; //数据库表名前缀
        public site_oauth(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }

        #region 基本方法================================
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" S.*,A.title as ptitle,A.img_url,A.remark,A.api_path,A.is_lock");
            strSql.Append(" FROM " + databaseprefix + "oauth_app as A INNER JOIN " + databaseprefix + "site_oauth as S ON A.id=S.oauth_id");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by S.sort_id asc,S.id desc");
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM(");
            strSql.Append("select S.*,A.title as ptitle,A.img_url,A.remark,A.api_path,A.is_lock");
            strSql.Append(" from " + databaseprefix + "oauth_app as A INNER JOIN " + databaseprefix + "site_oauth as S ON A.id=S.oauth_id");
            strSql.Append(") as temp_oauth");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return ReadDataBase.QueryFillDataSet(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 返回标题名称
        /// </summary>
        public string GetTitle(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 title from " + databaseprefix + "site_oauth");
            strSql.Append(" where id=" + id);
            string title = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }
            return string.Empty;
        }
        #endregion
    }
}