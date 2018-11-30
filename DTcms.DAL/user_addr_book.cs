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
    /// 数据访问类:用户地址簿
    /// </summary>
    public partial class user_addr_book : DapperRepository<Model.user_addr_book>
    {
        private string databaseprefix; //数据库表名前缀
        public user_addr_book(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.user_addr_book> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + "user_addr_book");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return GetModelList(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id, string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "user_addr_book");
            strSql.Append(" where id=@0 and user_name=@1");

            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), id, user_name) > 0;
        }

        /// <summary>
        /// 根据用户名删除一条数据
        /// </summary>
        public bool Delete(int id, string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "user_addr_book ");
            strSql.Append(" where id=@0 and user_name=@1");

            int rows = WriteDataBase.Execute(strSql.ToString(), id, user_name);
            return rows > 0;
        }
        #endregion

        #region 扩展方法============================
        /// <summary>
        /// 返回默认的地址
        /// </summary>
        public Model.user_addr_book GetDefault(string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 *");
            strSql.Append(" from " + databaseprefix + "user_addr_book ");
            strSql.Append(" where user_name=@0");

            return ReadDataBase.Query<Model.user_addr_book>(strSql.ToString(), user_name).FirstOrDefault();
        }

        /// <summary>
        /// 设置为默认的收货地址
        /// </summary>
        public void SetDefault(int id, string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + databaseprefix + "user_addr_book set is_default=0");
            strSql.Append(" where user_name=@0");
            WriteDataBase.Execute(strSql.ToString(), user_name);

            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("update " + databaseprefix + "user_addr_book set is_default=1");
            strSql2.Append(" where id=@0 and user_name=@1");

            WriteDataBase.Execute(strSql2.ToString(), id, user_name);
        }
        #endregion
    }
}