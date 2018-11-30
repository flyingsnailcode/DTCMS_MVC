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
    /// 扩展属性数据访问类:article_attribute_field
    /// </summary>
    public partial class article_attribute_field : DapperRepository<Model.article_attribute_field>
    {
        private string databaseprefix; //数据库表名前缀
        public article_attribute_field(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 查询是否存在列
        /// </summary>
        public bool Exists(string column_name)
        {
            //检查是否与文章字段相同
            Model.article artModel = new Model.article();
            //利用反射获得属性的所有公共属性
            Type modelType = artModel.GetType();
            PropertyInfo[] proInfo = modelType.GetProperties();
            foreach (PropertyInfo pi in proInfo)
            {
                if (pi.Name.ToLower() == column_name.ToLower())
                {
                    return true;
                }
            }
            //检查是否与扩展字段表列相同
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  " + databaseprefix + "article_attribute_field");
            strSql.Append(" where name=@0 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), column_name) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            Model.article_attribute_field model = GetModel(id);//取得扩展字段实体
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //删除所关联的频道数据表相关列
                        DataTable dt = new DAL.site_channel(databaseprefix).GetFieldList(conn, trans, id).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                //检查有无该频道数据表和列
                                int rowsCount = ReadDataBase.ExecuteScalar<int>(conn, trans, "select count(1) from syscolumns where id=object_id('" + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dr["name"].ToString() + "') and name='" + model.name + "'");
                                if (rowsCount > 0)
                                {
                                    //删除频道数据表一列
                                    WriteDataBase.Execute(conn, trans, "alter table " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dr["name"].ToString() + " drop column " + model.name);
                                }
                            }
                        }

                        //删除频道关联字段表
                        StringBuilder strSql1 = new StringBuilder();
                        strSql1.Append("delete from " + databaseprefix + "site_channel_field");
                        strSql1.Append(" where field_id=@0");
                        WriteDataBase.Execute(conn, trans, strSql1.ToString(), id);

                        //删除扩展字段主表
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("delete from " + databaseprefix + "article_attribute_field");
                        strSql.Append(" where id=@0");
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), id);

                        trans.Commit();//提交事务
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();//回滚事务
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM  " + databaseprefix + "article_attribute_field");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 获得频道对应的数据
        /// </summary>
        public DataSet GetList(int channel_id, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select A.* ");
            strSql.Append(" FROM " + databaseprefix + "article_attribute_field as A INNER JOIN " + databaseprefix + "site_channel_field as S ON A.id=S.field_id");
            strSql.Append(" where S.channel_id=" + channel_id);
            if (strWhere.Trim() != "")
            {
                strSql.Append(" and " + strWhere);
            }
            strSql.Append(" order by A.is_sys desc,A.sort_id asc,A.id desc");
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public override Model.article_attribute_field GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.article_attribute_field model = new Model.article_attribute_field();
            //利用反射获得属性的所有公共属性
            Type modelType = model.GetType();
            PropertyInfo[] pros = modelType.GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "article_attribute_field");
            strSql.Append(" where id="+ id);
            DataTable dt = WriteDataBase.QueryFillDataSet(strSql.ToString()).Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows[0].Table.Columns.Count; i++)
                {
                    //查找实体是否存在列表相同的公共属性
                    PropertyInfo proInfo = modelType.GetProperty(dt.Rows[0].Table.Columns[i].ColumnName);
                    if (proInfo != null && dt.Rows[0][i] != DBNull.Value)
                    {
                        proInfo.SetValue(model, dt.Rows[0][i], null);//用索引值设置属性值
                    }
                }
                return model;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.article_attribute_field model)
        {
            int i = 0;
            Model.article_attribute_field oldModel = GetModel(model.id);//取到旧的数据
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //修改主表信息
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("update  " + databaseprefix + "article_attribute_field set ");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键则追加sql字符串
                            if (!pi.Name.Equals("id"))
                            {
                                //判断属性值是否为空
                                if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                                {
                                    str1.Append(pi.Name + "=@" + i + ",");//声明参数
                                    i++;
                                    paras.Add(pi.GetValue(model, null));//对参数赋值
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(" where id=@" + i + " ");
                        paras.Add(model.id);
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());

                        //检查字段名和类型有无变化
                        if (oldModel.name.ToLower() != model.name.ToLower() || oldModel.data_type.ToLower() != model.data_type.ToLower())
                        {
                            DataTable dt = new DAL.site_channel(databaseprefix).GetFieldList(conn, trans, model.id).Tables[0];
                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    //检查有无该频道数据表和列
                                    int rowsCount = ReadDataBase.ExecuteScalar<int>(conn, trans, "select count(1) from syscolumns where id=object_id('" + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dr["name"].ToString() + "') and name='" + oldModel.name + "'");
                                    if (rowsCount > 0)
                                    {
                                        //修改列数据类型
                                        if (oldModel.data_type.ToLower() != model.data_type.ToLower())
                                        {
                                            WriteDataBase.Execute(conn, trans, "alter table " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dr["name"].ToString() + " alter column " + oldModel.name + " " + model.data_type);
                                        }
                                        //修改列名
                                        if (oldModel.name.ToLower() != model.name.ToLower())
                                        {
                                            WriteDataBase.Execute(conn, trans, "exec sp_rename '" + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dr["name"].ToString() + "." + oldModel.name + "','" + model.name + "','column'");
                                        }
                                    }
                                }
                            }
                        }

                        trans.Commit();//提交事务
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();//回滚事务
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region 扩展方法========================
        /// <summary>
        /// 获取扩展字段对称值
        /// </summary>
        public Dictionary<string, string> GetFields(int channel_id, int article_id, string strWhere)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            DataTable dt = GetList(channel_id, strWhere).Tables[0];
            if (dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append(dr["name"].ToString() + ",");
                }
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select top 1 " + Utils.DelLastComma(sb.ToString()) + " from " + databaseprefix + "article_attribute_value ");
                strSql.Append(" where article_id=" + article_id);
                DataSet ds = ReadDataBase.QueryFillDataSet(strSql.ToString());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (ds.Tables[0].Rows[0][dr["name"].ToString()] != null)
                        {
                            dic.Add(dr["name"].ToString(), ds.Tables[0].Rows[0][dr["name"].ToString()].ToString());
                        }
                        else
                        {
                            dic.Add(dr["name"].ToString(), "");
                        }
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 获取频道的扩展字段
        /// </summary>
        public Dictionary<string, string> GetFields(int channel_id)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            DataTable dt = GetList(channel_id, string.Empty).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dic.Add(dr["name"].ToString(), string.Empty);
                }
            }
            return dic;
        }
        #endregion

        #region 私有方法===============================
        /// <summary>
        /// 将对象转换为实体
        /// </summary>
        public Model.article_attribute_field DataRowToModel(DataRow row)
        {
            Model.article_attribute_field model = new Model.article_attribute_field();
            if (row != null)
            {
                #region 主表信息======================
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
                #endregion
            }
            return model;
        }

        #endregion
    }
}