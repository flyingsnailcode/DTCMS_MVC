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
    /// 数据访问类:请求回复规格
	/// </summary>
	public class weixin_request_rule : DapperRepository<Model.weixin_request_rule>
    {
        private string databaseprefix; //数据库表名前缀
        public weixin_request_rule(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            List<string> sqllist = new List<string>();
            //删除规则内容表
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("delete from " + databaseprefix + "weixin_request_content");
            strSql2.Append(" where rule_id=@0 ");
            sqllist.Add(strSql2.ToString());

            //删除规则主表
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "weixin_request_rule");
            strSql.Append(" where id=@0");
            sqllist.Add(strSql.ToString());

            return WriteDataBase.ExecuteSqlTran(sqllist, id) >0;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.weixin_request_rule GetModel(int account_id, int request_type)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.weixin_request_rule model = new Model.weixin_request_rule();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                //拼接字段，忽略List<T>
                if (!p.Name.Equals("values") && !p.Name.Equals("contents"))
                {
                    str1.Append(p.Name + ",");
                }
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "weixin_request_rule");
            strSql.AppendFormat(" where account_id={0} and request_type='{1}'", account_id, request_type);

            DataSet ds = WriteDataBase.QueryFillDataSet(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
		/// 增加一条数据
		/// </summary>
		public int Add(Model.weixin_request_rule model)
        {
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 主表信息==========================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();//数据字段
                        StringBuilder str2 = new StringBuilder();//数据参数
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("insert into " + databaseprefix + "weixin_request_rule(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键或List<T>则追加sql字符串
                            if (!pi.Name.Equals("id") && !pi.Name.Equals("contents"))
                            {
                                //判断属性值是否为空
                                if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                                {
                                    str1.Append(pi.Name + ",");//拼接字段
                                    str2.Append("@" + i + ",");//声明参数
                                    i++;
                                    paras.Add(pi.GetValue(model, null));//对参数赋值
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(") values (");
                        strSql.Append(str2.ToString().Trim(','));
                        strSql.Append(") ");
                        strSql.Append(";SELECT @@@IDENTITY;");
                        object obj = WriteDataBase.ExecuteScalar<object>(conn, trans, strSql.ToString(), paras.ToArray());
                        model.id = Convert.ToInt32(obj);
                        #endregion

                        #region 规则内容表信息====================
                        if (model.contents != null)
                        {
                            i = 0;
                            StringBuilder strSql2; //SQL字符串
                            StringBuilder str21; //数据库字段
                            StringBuilder str22; //声明参数
                            foreach (Model.weixin_request_content modelt in model.contents)
                            {
                                strSql2 = new StringBuilder();
                                str21 = new StringBuilder();
                                str22 = new StringBuilder();
                                PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                List<object> paras2 = new List<object>();
                                strSql2.Append("insert into " + databaseprefix + "weixin_request_content(");
                                foreach (PropertyInfo pi in pros2)
                                {
                                    if (!pi.Name.Equals("id"))
                                    {
                                        if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                        {
                                            str21.Append(pi.Name + ",");
                                            str22.Append("@" + i + ",");
                                            i++;
                                            if (pi.Name.Equals("rule_id"))
                                            {
                                                paras2.Add(model.id); //将刚插入的父ID赋值
                                            }
                                            else
                                            {
                                                paras2.Add(pi.GetValue(modelt, null));
                                            }
                                        }
                                    }
                                }
                                strSql2.Append(str21.ToString().Trim(','));
                                strSql2.Append(") values (");
                                strSql2.Append(str22.ToString().Trim(','));
                                strSql2.Append(") ");
                                WriteDataBase.Execute(conn, trans, strSql2.ToString(), paras2.ToArray());
                            }
                        }
                        #endregion

                        trans.Commit();//提交事务
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();//回滚事务
                        return 0;
                    }
                }
            }
            return model.id;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public override Model.weixin_request_rule GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.weixin_request_rule model = new Model.weixin_request_rule();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                //拼接字段，忽略List<T>
                if (!p.Name.Equals("values") && !p.Name.Equals("contents"))
                {
                    str1.Append(p.Name + ",");
                }
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "weixin_request_rule");
            strSql.Append(" where id="+ id);
            DataTable dt = WriteDataBase.QueryFillDataSet(strSql.ToString()).Tables[0];

            if (dt.Rows.Count > 0)
            {
                return DataRowToModel(dt.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
		/// 更新一条数据
		/// </summary>
        public bool Update(Model.weixin_request_rule model)
        {
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 主表信息==========================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("update  " + databaseprefix + "weixin_request_rule set ");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键或List<T>则追加sql字符串
                            if (!pi.Name.Equals("id") && !typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
                            {
                                //判断属性值是否为空
                                if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                                {
                                    //声明参数
                                    str1.Append(pi.Name + "=@" + i + ",");
                                    i++;
                                    //对参数赋值
                                    paras.Add(pi.GetValue(model, null));
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(" where id=@" + i + " ");
                        paras.Add(model.id);
                        WriteDataBase.Execute(conn, trans,strSql.ToString(), paras.ToArray());
                        #endregion

                        #region 规则内容表信息====================
                        //先删除的规则内容
                        WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "weixin_request_content where rule_id=" + model.id);
                        //重新添加规则内容
                        if (model.contents != null)
                        {
                            i = 0;
                            StringBuilder strSql2; //SQL字符串
                            StringBuilder str21; //数据库字段
                            StringBuilder str22; //声明参数
                            foreach (Model.weixin_request_content modelt in model.contents)
                            {
                                strSql2 = new StringBuilder();
                                str21 = new StringBuilder();
                                str22 = new StringBuilder();
                                PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                List<object> paras2 = new List<object>();
                                strSql2.Append("insert into " + databaseprefix + "weixin_request_content(");
                                foreach (PropertyInfo pi in pros2)
                                {
                                    if (!pi.Name.Equals("id"))
                                    {
                                        if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                        {
                                            str21.Append(pi.Name + ",");
                                            str22.Append("@" + i + ",");
                                            i++;
                                            if (pi.Name.Equals("rule_id"))
                                            {
                                                paras2.Add(model.id); //将规则ID赋值
                                            }
                                            else
                                            {
                                                paras2.Add(pi.GetValue(modelt, null));
                                            }
                                        }
                                    }
                                }
                                strSql2.Append(str21.ToString().Trim(','));
                                strSql2.Append(") values (");
                                strSql2.Append(str22.ToString().Trim(','));
                                strSql2.Append(") ");
                                WriteDataBase.Execute(conn, trans, strSql2.ToString(), paras2.ToArray());
                            }
                        }
                        #endregion

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region 微信通讯方法============================
        /// <summary>
        /// 得到规则ID以及回复类型
        /// </summary>
        public int GetRuleIdAndResponseType(int account_id, string strWhere, out int response_type)
        {
            int rule_id = 0;
            response_type = 0;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 id,response_type from " + databaseprefix + "weixin_request_rule");
            strSql.Append(" where account_id=" + account_id);
            if (strWhere != null && strWhere.Length > 0)
            {
                strSql.Append(" and " + strWhere);
            }
            SqlDataReader sr = ReadDataBase.ExecuteScalar<SqlDataReader>(strSql.ToString());

            while (sr.Read())
            {
                rule_id = int.Parse(sr["id"].ToString());
                response_type = int.Parse(sr["response_type"].ToString());
            }
            sr.Close();

            return rule_id;
        }

        /// <summary>
        /// 得到关健字查询的规则ID及回复类型(如需提高效率可使用存储过程)
        /// </summary>
        public int GetKeywordsRuleId(int account_id, string keywords, out int response_type)
        {
            int rule_id = 0;
            //精确匹配
            StringBuilder strSql3 = new StringBuilder();
            strSql3.Append("select top 1 id,response_type from " + databaseprefix + "weixin_request_rule");
            strSql3.Append(" where account_id=" + account_id + " and request_type=1");
            strSql3.Append(" and(keywords like '" + keywords + "|%' or keywords='%|" + keywords + "' or keywords like '%|" + keywords + "|%' or keywords='" + keywords + "')");
            strSql3.Append(" order by sort_id asc,add_time desc");
            DataSet ds3 = WriteDataBase.QueryFillDataSet(strSql3.ToString());
            if (ds3.Tables[0].Rows.Count > 0)
            {
                rule_id = int.Parse(ds3.Tables[0].Rows[0][0].ToString());
                response_type = int.Parse(ds3.Tables[0].Rows[0][1].ToString());
                return rule_id;
            }
            //模糊匹配
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("select top 1 id,response_type from " + databaseprefix + "weixin_request_rule");
            strSql2.Append(" where account_id=" + account_id + " and request_type=1 and keywords like '%" + keywords + "%'");
            strSql2.Append(" order by sort_id asc,add_time desc");
            DataSet ds2 = WriteDataBase.QueryFillDataSet(strSql2.ToString());
            if (ds2.Tables[0].Rows.Count > 0)
            {
                rule_id = int.Parse(ds2.Tables[0].Rows[0][0].ToString());
                response_type = int.Parse(ds2.Tables[0].Rows[0][1].ToString());
                return rule_id;
            }
            //默认回复
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("select top 1 id,response_type from " + databaseprefix + "weixin_request_rule");
            strSql1.Append(" where account_id=" + account_id + " and request_type=0");
            strSql1.Append(" order by sort_id asc,add_time desc");
            DataSet ds1 = WriteDataBase.QueryFillDataSet(strSql1.ToString());
            if (ds1.Tables[0].Rows.Count > 0)
            {
                rule_id = int.Parse(ds1.Tables[0].Rows[0][0].ToString());
                response_type = int.Parse(ds1.Tables[0].Rows[0][1].ToString());
                return rule_id;
            }
            response_type = 0;
            return rule_id;
        }
        #endregion

        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.weixin_request_rule DataRowToModel(DataRow row)
        {
            Model.weixin_request_rule model = new Model.weixin_request_rule();
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

                #region 子表信息======================
                StringBuilder strSql1 = new StringBuilder();
                strSql1.Append("select * from " + databaseprefix + "weixin_request_content");
                strSql1.Append(" where rule_id="+ model.id);
                DataTable dt1 = WriteDataBase.QueryFillDataSet(strSql1.ToString()).Tables[0];

                if (dt1.Rows.Count > 0)
                {
                    int rowsCount = dt1.Rows.Count;
                    List<Model.weixin_request_content> models = new List<Model.weixin_request_content>();
                    Model.weixin_request_content modelt;
                    for (int n = 0; n < rowsCount; n++)
                    {
                        modelt = new Model.weixin_request_content();
                        Type modeltType = modelt.GetType();
                        for (int i = 0; i < dt1.Rows[n].Table.Columns.Count; i++)
                        {
                            PropertyInfo proInfo = modeltType.GetProperty(dt1.Rows[n].Table.Columns[i].ColumnName);
                            if (proInfo != null && dt1.Rows[n][i] != DBNull.Value)
                            {
                                proInfo.SetValue(modelt, dt1.Rows[n][i], null);
                            }
                        }
                        models.Add(modelt);
                    }
                    model.contents = models;
                }
                #endregion
            }
            return model;
        }
    }
}

