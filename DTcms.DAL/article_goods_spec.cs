using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Reflection;

namespace DTcms.DAL
{
    /// <summary>
    /// 商品对应规格
    /// </summary>
    public partial class article_goods_spec : DapperRepository<Model.article_goods_spec>
    {
        private string databaseprefix; //数据库表名前缀
        public article_goods_spec(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================

        /// <summary>
        /// 增加一条数据，带事务
        /// </summary>
        public int Add(IDbConnection conn, IDbTransaction trans, Model.article_goods_spec model, int channel_id, int article_id)
        {
            int i = 0;
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<object> paras = new List<object>();
            strSql.Append("insert into " + databaseprefix + "article_goods_spec(");
            foreach (PropertyInfo pi in pros)
            {
                //判断属性值是否为空
                if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                {
                    str1.Append(pi.Name + ",");//拼接字段
                    str2.Append("@" + i + ",");//声明参数
                    i++;
                    switch (pi.Name)
                    {
                        case "channel_id":
                            paras.Add(channel_id);
                            break;
                        case "article_id":
                            paras.Add(article_id);
                            break;
                        default:
                            paras.Add(pi.GetValue(model, null));//对参数赋值
                            break;
                    }
                }
            }
            strSql.Append(str1.ToString().Trim(','));
            strSql.Append(") values (");
            strSql.Append(str2.ToString().Trim(','));
            strSql.Append(") ");
            strSql.Append(";SELECT @@@IDENTITY;");
            object obj = WriteDataBase.ExecuteScalar<object>(conn, trans, strSql.ToString(), paras.ToArray());
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 删除一条数据，带事务
        /// </summary>
        public bool Delete(IDbConnection conn, IDbTransaction trans, int channel_id, int article_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "article_goods_spec");
            strSql.Append(" where channel_id=@0 and article_id=@1");
            return WriteDataBase.Execute(strSql.ToString(), channel_id, article_id) > 0;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Model.article_goods_spec> GetList(int channel_id, int article_id, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + "article_goods_spec");
            strSql.Append(" where channel_id=" + channel_id + " and article_id=" + article_id);
            if (strWhere.Trim() != "")
            {
                strSql.Append(" and " + strWhere);
            }
            
            DataTable dt = ReadDataBase.QueryFillDataSet(strSql.ToString()).Tables[0];
            List<Model.article_goods_spec> modelList = new List<Model.article_goods_spec>();
            if (dt.Rows.Count > 0)
            {
                for (int n = 0; n < dt.Rows.Count; n++)
                {
                    Model.article_goods_spec model = new Model.article_goods_spec();
                    //利用反射获得属性的所有公共属性
                    Type modelType = model.GetType();
                    for (int i = 0; i < dt.Rows[n].Table.Columns.Count; i++)
                    {
                        //查找实体是否存在列表相同的公共属性
                        PropertyInfo proInfo = modelType.GetProperty(dt.Rows[n].Table.Columns[i].ColumnName);
                        if (proInfo != null && dt.Rows[n][i] != DBNull.Value)
                        {
                            proInfo.SetValue(model, dt.Rows[n][i], null);//用索引值设置属性值
                        }
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }
        #endregion
    }
}
