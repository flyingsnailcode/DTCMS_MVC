using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using DTcms.DBUtility;
using DTcms.Common;
using System.Linq;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:站点支付方式表
    /// </summary>
    public partial class site_payment : DapperRepository<Model.site_payment>
    {
        private string databaseprefix; //数据库表名前缀
        public site_payment(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }

        #region 基本方法================================
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM(");
            strSql.Append("select S.*,P.img_url,P.title as ptitle,P.remark,P.type,P.poundage_type,P.poundage_amount,P.redirect_url,P.return_url,P.notify_url,P.is_lock");
            strSql.Append(" from " + databaseprefix + "payment as P INNER JOIN " + databaseprefix + "site_payment as S ON P.id=S.payment_id");
            strSql.Append(") as temp_payment");
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
        /// 将对象转换实体
        /// </summary>
        public Model.site_payment DataRowToModel(DataRow row)
        {
            Model.site_payment model = new Model.site_payment();
            if (row != null)
            {
                //利用反射获得属性的所有公共属性
                Type modelType = model.GetType();
                for (int i = 0; i < row.Table.Columns.Count; i++)
                {
                    //查找实体是否存在列表相同的公共属性
                    PropertyInfo proInfo = modelType.GetProperty(row.Table.Columns[i].ColumnName);
                    if (proInfo != null && row[i] != DBNull.Value)
                    {
                        proInfo.SetValue(model, row[i], null);//用索引值设置属性值
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 获取支付平台实体
        /// </summary>
        public Model.payment GetPaymentModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 P.* FROM " + databaseprefix + "payment as P INNER JOIN " + databaseprefix + "site_payment as S ON P.id=S.payment_id");
            strSql.Append(" and S.id=@0 ");
            return ReadDataBase.Query<Model.payment>(strSql.ToString(), id).FirstOrDefault();
        }
        #endregion
    }
}