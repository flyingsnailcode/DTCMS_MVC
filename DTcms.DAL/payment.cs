using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:支付方式
    /// </summary>
    public partial class payment : DapperRepository<Model.payment>
    {
        private string databaseprefix; //数据库表名前缀
        public payment(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 基本方法
        /// <summary>
        /// 返回标题名称
        /// </summary>
        public string GetTitle(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 title from " + databaseprefix + "site_payment");
            strSql.Append(" where id=" + id);
            string title = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }
            return string.Empty;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            //删除站点支付方式
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("delete from " + databaseprefix + "site_payment");
            strSql1.Append(" where payment_id=@0");
            WriteDataBase.Execute(strSql1.ToString(), id);

            //删除主表
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "payment");
            strSql.Append(" where id=@0");
            int rows=WriteDataBase.Execute(strSql.ToString(), id);

            return rows > 0;
        }

        #endregion
        #region 扩展方法================================
        /// <summary>
        /// 获取站点未添加数据
        /// </summary>
        public DataSet GetList(int site_id, int payment_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM  " + databaseprefix + "payment");
            strSql.Append(" where is_lock=0 and id not in(");
            strSql.Append("select payment_id from " + databaseprefix + "site_payment where site_id=" + site_id);
            if (payment_id > 0)
            {
                strSql.Append(" and payment_id<>" + payment_id);
            }
            strSql.Append(")");
            strSql.Append(" order by sort_id asc,id desc");
            return WriteDataBase.QueryFillDataSet(strSql.ToString());
        }
        #endregion
    }
}