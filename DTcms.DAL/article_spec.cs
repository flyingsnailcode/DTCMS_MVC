using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Reflection;
using System.Linq;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:规格
    /// </summary>
    public partial class article_spec : DapperRepository<Model.article_spec>
    {
        private string databaseprefix; //数据库表名前缀
        public article_spec(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int id)
        {
            //删除规格值
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("delete from " + databaseprefix + "article_spec");
            strSql1.Append(" where parent_id=@0");
           WriteDataBase.Execute(strSql1.ToString(), id);

            //删除规格
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "article_spec ");
            strSql.Append(" where id=@0");
            int rowsAffected = WriteDataBase.Execute(strSql.ToString(), id);
            return rowsAffected > 0;
        }

        /// <summary>
		/// 增加一条数据
		/// </summary>
		public int Add(Model.article_spec model)
        {
            int i = 0;
            try
            {
                #region 主表信息==========================
                StringBuilder strSql = new StringBuilder();
                //数据字段
                StringBuilder str1 = new StringBuilder();
                //数据参数
                StringBuilder str2 = new StringBuilder();
                //利用反射获得属性的所有公共属性
                PropertyInfo[] pros = model.GetType().GetProperties();
                List<object> paras = new List<object>();
                strSql.Append("insert into " + databaseprefix + "article_spec(");
                foreach (PropertyInfo pi in pros)
                {
                    //如果不是主键或List<T>则追加sql字符串
                    if (!pi.Name.Equals("id") && !typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
                    {
                        //判断属性值是否为空
                        if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                        {
                            //拼接字段
                            str1.Append(pi.Name + ",");
                            //声明参数
                            str2.Append("@" + i + ",");
                            i++;
                            //对参数赋值
                            paras.Add(pi.GetValue(model, null));
                        }
                    }
                }
                strSql.Append(str1.ToString().Trim(','));
                strSql.Append(") values (");
                strSql.Append(str2.ToString().Trim(','));
                strSql.Append(") ");
                strSql.Append(";SELECT @@@IDENTITY;");
                object obj = WriteDataBase.ExecuteScalar<object>(strSql.ToString(), paras.ToArray());
                model.id = Convert.ToInt32(obj);
                #endregion

                #region 规格值信息========================
                if (model.values != null)
                {
                    i = 0;
                    StringBuilder strSql2;//SQL字符串
                    StringBuilder str21;//数据库字段
                    StringBuilder str22;//声明参数
                    foreach (Model.article_spec_value modelt in model.values)
                    {
                        strSql2 = new StringBuilder();
                        str21 = new StringBuilder();
                        str22 = new StringBuilder();
                        PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                        List<object> paras2 = new List<object>();
                        strSql2.Append("insert into " + databaseprefix + "article_spec(");
                        foreach (PropertyInfo pi in pros2)
                        {
                            if (!pi.Name.Equals("id"))
                            {
                                if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                {
                                    str21.Append(pi.Name + ",");
                                    str22.Append("@" + i + ",");
                                    i++;
                                    if (pi.Name.Equals("parent_id"))
                                    {
                                        paras2.Add(model.id);//将刚插入的父ID赋值
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
                        WriteDataBase.Execute(strSql2.ToString(), paras2.ToArray());
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                return 0;
            }
            return model.id;
        }


        /// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Model.article_spec model)
        {
            int i = 0;
            try
            {
                #region 主表信息==========================
                StringBuilder strSql = new StringBuilder();
                StringBuilder str1 = new StringBuilder();
                //利用反射获得属性的所有公共属性
                PropertyInfo[] pros = model.GetType().GetProperties();
                List<object> paras = new List<object>();
                strSql.Append("update  " + databaseprefix + "article_spec set ");
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
                WriteDataBase.Execute(strSql.ToString(), paras.ToArray());
                #endregion

                #region 规格值信息========================
                //删除已删除的规格值
                DeleteValues(model.values, model.id);
                //添加/修改规格值
                if (model.values != null)
                {
                    StringBuilder strSql2; //SQL字符串
                    StringBuilder str21; //数据库字段
                    StringBuilder str22; //声明参数
                    foreach (Model.article_spec_value modelt in model.values)
                    {
                        strSql2 = new StringBuilder();
                        str21 = new StringBuilder();
                        str22 = new StringBuilder();
                        PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                        List<object> paras2 = new List<object>();
                        if (modelt.id > 0)
                        {
                            i = 0;
                            strSql2.Append("update " + databaseprefix + "article_spec set ");
                            foreach (PropertyInfo pi in pros2)
                            {
                                //如果不是主键则追加sql字符串
                                if (!pi.Name.Equals("id"))
                                {
                                    //判断属性值是否为空
                                    if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                    {
                                        str21.Append(pi.Name + "=@" + i + ",");
                                        i++;
                                        paras2.Add(pi.GetValue(modelt, null));
                                    }
                                }
                            }
                            strSql2.Append(str21.ToString().Trim(','));
                            strSql2.Append(" where id=@" + i + " ");
                            paras2.Add(modelt.id);
                            WriteDataBase.Execute(strSql2.ToString(), paras2.ToArray());
                        }
                        else
                        {
                            i = 0;
                            strSql2.Append("insert into " + databaseprefix + "article_spec(");
                            foreach (PropertyInfo pi in pros2)
                            {
                                if (!pi.Name.Equals("id"))
                                {
                                    if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                    {
                                        str21.Append(pi.Name + ",");
                                        str22.Append("@" + i + ",");
                                        i++;
                                        paras2.Add(pi.GetValue(modelt, null));
                                    }
                                }
                            }
                            strSql2.Append(str21.ToString().Trim(','));
                            strSql2.Append(") values (");
                            strSql2.Append(str22.ToString().Trim(','));
                            strSql2.Append(") ");
                            WriteDataBase.Execute(strSql2.ToString(), paras2.ToArray());
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
		/// 得到一个对象实体
		/// </summary>
        public override Model.article_spec GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 id,parent_id,title,remark,sort_id from " + databaseprefix + "article_spec");
            strSql.Append(" where id=@0");

            #region 主表信息======================
            Model.article_spec model = ReadDataBase.Query<Model.article_spec>(strSql.ToString(), id).FirstOrDefault();
            #endregion

            #region 子表信息======================
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("select * from " + databaseprefix + "article_spec");
            strSql1.Append(" where parent_id="+ model.id);
            
            DataTable dt1 = ReadDataBase.QueryFillDataSet(strSql1.ToString()).Tables[0];
            if (dt1.Rows.Count > 0)
            {
                int rowsCount = dt1.Rows.Count;
                List<Model.article_spec_value> models = new List<Model.article_spec_value>();
                Model.article_spec_value modelt;
                for (int n = 0; n < rowsCount; n++)
                {
                    modelt = new Model.article_spec_value();
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
                model.values = models;
            }
            #endregion

            return model;
        }
        #endregion

        #region 私有方法========================
        /// <summary>
        /// 返回栏目规格列表
        /// </summary>
        public DataSet GetCategorySpecList(int category_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select S.* FROM " + databaseprefix + "article_spec as S INNER JOIN " + databaseprefix + "article_category_spec as C");
            strSql.Append(" ON S.id=C.spec_id and C.category_id=" + category_id);
            strSql.Append(" order by sort_id asc,S.id desc");
            return WriteDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 根据频道和分类ID筛选出规格(慎用)
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="category_id">类别ID</param>
        /// <returns>DataSet</returns>
        public DataSet GetParentList(string channel_name, int category_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from " + databaseprefix + "article_spec where id in(");
            strSql.Append("select spec_id from " + databaseprefix + "article_goods_spec where parent_id=0 and article_id in(");
            strSql.Append("select id from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name + " where category_id in(");
            strSql.Append("select id from " + databaseprefix + "article_category");
            if (category_id > 0)
            {
                strSql.Append(" where class_list like '%," + category_id + ",%'");
            }
            strSql.Append(")) group by spec_id)");
            strSql.Append(" order by sort_id asc");
            return WriteDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 查找不存在的规格值并删除
        /// </summary>
        private void DeleteValues(List<Model.article_spec_value> models, int parentId)
        {
            StringBuilder idList = new StringBuilder();
            if (models != null)
            {
                foreach (Model.article_spec_value modelt in models)
                {
                    if (modelt.id > 0)
                    {
                        idList.Append(modelt.id + ",");
                    }
                }
                string id_list = Utils.DelLastChar(idList.ToString(), ",");
                if (!string.IsNullOrEmpty(id_list))
                {
                    WriteDataBase.Execute("delete from " + databaseprefix + "article_spec where parent_id=" + parentId + " and id not in(" + id_list + ")");
                }
            }
        }
        /// <summary>
        /// 查找不存在的规格值并删除
        /// </summary>
        private void DeleteList(List<Model.article_spec_value> models, int parentId)
        {
            StringBuilder idList = new StringBuilder();
            if (models != null)
            {
                foreach (Model.article_spec_value modelt in models)
                {
                    if (modelt.id > 0)
                    {
                        idList.Append(modelt.id + ",");
                    }
                }
                string id_list = Utils.DelLastChar(idList.ToString(), ",");
                if (!string.IsNullOrEmpty(id_list))
                {
                    WriteDataBase.Execute("delete from " + databaseprefix + "article_spec where parent_id=" + parentId + " and id not in(" + id_list + ")");
                }
            }
        }
        #endregion
    }
}

