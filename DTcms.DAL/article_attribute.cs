using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Reflection;

namespace DTcms.DAL
{
    /// <summary>
    /// 扩展参数
    /// </summary>
    public partial class article_attribute : DapperRepository<Model.article_attribute>
    {
        private string databaseprefix; //数据库表名前缀
        public article_attribute(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Model.article_attribute> GetList(int channel_id, int article_id)
        {
            List<Model.article_attribute> modelList = new List<Model.article_attribute>();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + "article_attribute ");
            strSql.Append(" where channel_id=" + channel_id + " and article_id=" + article_id);
            DataTable dt = ReadDataBase.QueryFillDataSet(strSql.ToString()).Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int n = 0; n < dt.Rows.Count; n++)
                {
                    modelList.Add(DataRowToModel(dt.Rows[n]));
                }
            }
            return modelList;
        }

        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.article_attribute DataRowToModel(DataRow row)
        {
            Model.article_attribute model = new Model.article_attribute();
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
        /// 增加一条数据
        /// </summary>
        public void Add(IDbConnection conn, IDbTransaction trans, List<Model.article_attribute> models, int channel_id, int article_id)
        {
            int i = 0;
            if (models != null)
            {
                StringBuilder strSql;
                StringBuilder str1; ;//数据字段
                StringBuilder str2;//数据参数
                foreach (Model.article_attribute modelt in models)
                {
                    i = 0;
                    strSql = new StringBuilder();
                    str1 = new StringBuilder();
                    str2 = new StringBuilder();
                    //利用反射获得属性的所有公共属性
                    PropertyInfo[] pros = modelt.GetType().GetProperties();
                    List<object> paras = new List<object>();
                    strSql.Append("insert into " + databaseprefix + "article_attribute(");
                    foreach (PropertyInfo pi in pros)
                    {
                        //如果不是主键则追加sql字符串
                        if (!pi.Name.Equals("id"))
                        {
                            //判断属性值是否为空
                            if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
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
                                        paras.Add(article_id);//刚插入的文章ID
                                        break;
                                    default:
                                        paras.Add(pi.GetValue(modelt, null));//对参数赋值
                                        break;
                                }
                            }
                        }
                    }
                    strSql.Append(str1.ToString().Trim(','));
                    strSql.Append(") values (");
                    strSql.Append(str2.ToString().Trim(','));
                    strSql.Append(") ");
                    WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());//带事务
                }
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public void Update(IDbConnection conn, IDbTransaction trans, List<Model.article_attribute> models, int channel_id, int article_id)
        {
            int i = 0;
            //删除
            WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "article_attribute where channel_id=" + channel_id + " and article_id=" + article_id);
            // 添加
            if (models != null)
            {
                StringBuilder strSql;
                StringBuilder str1; ;//数据字段
                StringBuilder str2;//数据参数
                foreach (Model.article_attribute modelt in models)
                {
                    i = 0;
                    strSql = new StringBuilder();
                    str1 = new StringBuilder();
                    str2 = new StringBuilder();
                    //利用反射获得属性的所有公共属性
                    PropertyInfo[] pros = modelt.GetType().GetProperties();
                    List<object> paras = new List<object>();
                    strSql.Append("insert into " + databaseprefix + "article_attribute(");
                    foreach (PropertyInfo pi in pros)
                    {
                        //如果不是主键则追加sql字符串
                        if (!pi.Name.Equals("id"))
                        {
                            //判断属性值是否为空
                            if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                            {
                                str1.Append(pi.Name + ",");//拼接字段
                                str2.Append("@" +i + ",");//声明参数
                                i++;
                                switch (pi.Name)
                                {
                                    case "channel_id":
                                        paras.Add(channel_id);
                                        break;
                                    case "article_id":
                                        paras.Add(article_id);//刚插入的文章ID
                                        break;
                                    default:
                                        paras.Add(pi.GetValue(modelt, null));//对参数赋值
                                        break;
                                }
                            }
                        }
                    }
                    strSql.Append(str1.ToString().Trim(','));
                    strSql.Append(") values (");
                    strSql.Append(str2.ToString().Trim(','));
                    strSql.Append(") ");
                    WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());//带事务
                }
            }
        }
        #endregion
    }
}
