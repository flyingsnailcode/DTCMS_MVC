using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Dapper;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:内容类别
    /// </summary>
    public partial class article_category : DapperRepository<Model.article_category>
    {
        private string databaseprefix; //数据库表名前缀
        public article_category(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 取得指定类别下的列表
        /// </summary>
        /// <param name="parent_id">父ID</param>
        /// <param name="channel_id">频道ID</param>
        /// <returns></returns>
        public DataTable GetChildList(int parent_id, int channel_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from " + databaseprefix + "article_category");
            strSql.Append(" where channel_id=" + channel_id + " and parent_id=" + parent_id + " order by sort_id asc,id desc");
            DataSet ds = ReadDataBase.QueryFillDataSet(strSql.ToString());
            return ds.Tables[0];
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.article_category model, int role_id)
        {
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 添加主表数据====================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();//数据字段
                        StringBuilder str2 = new StringBuilder();//数据参数
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("insert into " + databaseprefix + "article_category(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键则追加sql字符串
                            if (!pi.Name.Equals("id"))
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

                        if (model.parent_id > 0)
                        {
                            Model.article_category model2 = GetModel(conn, trans, model.parent_id); //带事务
                            model.class_list = model2.class_list + model.id + ",";
                            model.class_layer = model2.class_layer + 1;
                        }
                        else
                        {
                            model.class_list = "," + model.id + ",";
                            model.class_layer = 1;
                        }
                        
                        //添加权限菜单
                        object name = ReadDataBase.ExecuteScalar<object>(conn, trans, "select top 1 name from [" + databaseprefix + "site_channel]  where id=" + model.channel_id); //带事务
                        if (null != name)
                        {
                            //自动分级
                            string parent_name = "channel_" + name.ToString() + "_category";
                            if (model.parent_id > 0)
                            {
                                parent_name += "_" + model.parent_id;
                            }
                            new DAL.navigation(databaseprefix).Add(conn, trans, parent_name, "channel_" + name.ToString() + "_category_" + model.id, model.title, "", model.sort_id, model.channel_id, "Show", 1);

                            if (role_id > 0)
                            {
                                Model.manager_role_value valModel = new Model.manager_role_value();
                                valModel.role_id = role_id;
                                valModel.nav_name = "channel_" + name.ToString() + "_category_" + model.id;
                                valModel.action_type = "Show";
                                new DAL.manager_role_value(databaseprefix).Add(valModel);
                            }
                        }

                        //修改节点列表和深度
                        WriteDataBase.Execute(conn, trans, "update " + databaseprefix + "article_category set class_list='" + model.class_list + "', class_layer=" + model.class_layer + " where id=" + model.id); //带事务
                        #endregion

                        #region 栏目规格===========================
                        if (model.category_specs != null)
                        {
                            StringBuilder strSql3;
                            foreach (Model.article_category_spec modelt in model.category_specs)
                            {
                                strSql3 = new StringBuilder();
                                strSql3.Append("insert into " + databaseprefix + "article_category_spec(");
                                strSql3.Append("category_id,spec_id)");
                                strSql3.Append(" values (");
                                strSql3.Append("@0,@1)");
                                WriteDataBase.Execute(conn, trans, strSql3.ToString(), model.id, modelt.spec_id);
                            }
                        }
                        #endregion
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return 0;
                    }
                }
            }
            return model.id;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.article_category model)
        {
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //先判断选中的父节点是否被包含
                        if (IsContainNode(model.id, model.parent_id))
                        {
                            //查找旧数据
                            Model.article_category oldModel = GetModel(model.id);
                            //查找旧父节点数据
                            string class_list = "," + model.parent_id + ",";
                            int class_layer = 1;
                            if (oldModel.parent_id > 0)
                            {
                                Model.article_category oldParentModel = GetModel(conn, trans, oldModel.parent_id); //带事务
                                class_list = oldParentModel.class_list + model.parent_id + ",";
                                class_layer = oldParentModel.class_layer + 1;
                            }
                            //先提升选中的父节点
                            WriteDataBase.Execute(conn, trans, "update " + databaseprefix + "article_category set parent_id=" + oldModel.parent_id + ",class_list='" + class_list + "', class_layer=" + class_layer + " where id=" + model.parent_id); //带事务
                            UpdateChilds(conn, trans, model.parent_id); //带事务
                        }
                        //更新子节点
                        if (model.parent_id > 0)
                        {
                            Model.article_category model2 = GetModel(conn, trans, model.parent_id); //带事务
                            model.class_list = model2.class_list + model.id + ",";
                            model.class_layer = model2.class_layer + 1;
                        }
                        else
                        {
                            model.class_list = "," + model.id + ",";
                            model.class_layer = 1;
                        }
                        #region 修改主表数据==========================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("update  " + databaseprefix + "article_category set ");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键则追加sql字符串
                            if (!pi.Name.Equals("id") && !typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
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

                        //首先判断菜单是否存在
                        object name = ReadDataBase.ExecuteScalar<object>(conn, trans, "select top 1 name from [" + databaseprefix + "site_channel]  where id=" + model.channel_id); //带事务
                        if (null != name)
                        {
                            //自动分级
                            string parent_name = "channel_" + name.ToString() + "_category";
                            if (model.parent_id > 0)
                            {
                                parent_name += "_" + model.parent_id;
                            }
                            if (ReadDataBase.ExecuteScalar<int>(conn, trans, "select count(*) as H from [" + databaseprefix + "navigation]  where name='" + "channel_" + name.ToString() + "_category_" + model.id + "'") == 0)
                            {
                                new DAL.navigation(databaseprefix).Add(conn, trans, parent_name, "channel_" + name.ToString() + "_category_" + model.id, model.title, "", model.sort_id, model.channel_id, "Show", 1);
                            }
                            else
                            {
                                int parent_id = ReadDataBase.ExecuteScalar<int>(conn, trans, "select top 1 id from [" + databaseprefix + "navigation]  where name='" + parent_name + "'");
                                if (parent_id > 0)
                                {
                                    new DAL.navigation(databaseprefix).Update(conn, trans, parent_name = "channel_" + name.ToString() + "_category_" + model.id, parent_id, parent_name, model.title, model.sort_id);
                                }
                            }
                        }

                        //更新子节点
                        UpdateChilds(conn, trans, model.id);

                        #endregion

                        #region 栏目规格===========================
                        //删除已删除的栏目规格
                        WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "article_category_spec where category_id="+ model.id);
                        //添加/修改栏目规格
                        if (model.category_specs != null)
                        {
                            StringBuilder strSql3;
                            foreach (Model.article_category_spec modelt in model.category_specs)
                            {
                                strSql3 = new StringBuilder();
                                if (modelt.id > 0)
                                {
                                    strSql3.Append("update " + databaseprefix + "article_category_spec ");
                                    strSql3.Append("category_id=@0,");
                                    strSql3.Append("spec_id=@1");
                                    strSql3.Append(" where id=@2");
                                    WriteDataBase.Execute(conn, trans, strSql3.ToString(), modelt.category_id, modelt.spec_id, modelt.id);
                                }
                                else
                                {
                                    strSql3.Append("insert into " + databaseprefix + "article_category_spec(");
                                    strSql3.Append("category_id,spec_id)");
                                    strSql3.Append(" values (");
                                    strSql3.Append("@0,@1)");
                                    WriteDataBase.Execute(conn, trans, strSql3.ToString(), model.id, modelt.spec_id);
                                }
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

        /// <summary>
        /// 验证节点是否被包含
        /// </summary>
        /// <param name="id">待查询的节点</param>
        /// <param name="parent_id">父节点</param>
        /// <returns></returns>
        private bool IsContainNode(int id, int parent_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "article_category where class_list like '%," + id + ",%' and id=" + parent_id);
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString()) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            Model.article_category model = GetModel(id);
            if (null != model)
            {
                bool resault = true;
                //修改以事件删除数据时，同时删时删除权限
                using (IDbConnection conn = new DapperView().Context())
                {
                    
                    using (IDbTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            //频道名称
                            object name = ReadDataBase.ExecuteScalar<object>(conn, trans, "select top 1 name from [" + databaseprefix + "site_channel]  where id=" + model.channel_id); //带事务
                            if (null != name)
                            {
                                //先判断选中是否存在子节点
                                if (ReadDataBase.ExecuteScalar<int>(conn, trans, "select count(1) from [" + databaseprefix + "article_category] where parent_id=" + model.id) > 0)
                                {
                                    WriteDataBase.Execute(conn, trans, "update [" + databaseprefix + "article_category] set parent_id=" + model.parent_id + " where parent_id=" + model.id); //带事务
                                    UpdateChilds(conn, trans, model.parent_id); //带事务

                                    //修改权限菜单
                                    Model.navigation modelt = new DAL.navigation(databaseprefix).GetModel(conn, trans, "channel_" + name.ToString() + "_category_" + model.id);
                                    if (null != modelt)
                                    {
                                        WriteDataBase.Execute(conn, trans, "update [" + databaseprefix + "navigation] set parent_id=" + modelt.parent_id + " where parent_id=" + modelt.id); //带事务
                                    }
                                }
                                //删除权限菜单
                                WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "navigation where channel_id=" + model.channel_id + " and name='" + "channel_" + name.ToString() + "_category_" + model.id + "'");//删除角色权限
                                WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "manager_role_value where nav_name='" + "channel_" + name.ToString() + "_category_" + model.id + "'");
                            }
                            //删除栏目规格
                            StringBuilder strSql2 = new StringBuilder();
                            strSql2.Append("delete from " + databaseprefix + "article_category_spec");
                            strSql2.Append(" where category_id=@0");
                            WriteDataBase.Execute(conn, trans, strSql2.ToString(), id);

                            //删除类别
                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("delete from " + databaseprefix + "article_category where id=@0");
                            WriteDataBase.Execute(conn, trans, strSql.ToString(), id);
                            
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            resault = false;
                            trans.Rollback();
                        }
                    }
                }
                if (resault)
                {
                    //删除图片
                    Utils.DeleteFile(model.img_url);
                }
                return resault;
            }
            return false;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public override Model.article_category GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from " + databaseprefix + "article_category ");
            strSql.Append(" where id=@0");
            return ReadDataBase.Query<Model.article_category>(strSql.ToString(), id).FirstOrDefault();
        }


        /// <summary>
        /// 得到一个对象实体(重载，带事务)
        /// </summary>
        public Model.article_category GetModel(IDbConnection conn, IDbTransaction trans, int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 * from " + databaseprefix + "article_category ");
            strSql.Append(" where id=" + id);
            DataSet ds = ReadDataBase.QueryFillDataSet(conn, trans, strSql.ToString());
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
        /// 修改子节点的ID列表及深度（自身迭代）
        /// </summary>
        /// <param name="parent_id"></param>
        private void UpdateChilds(IDbConnection conn, IDbTransaction trans, int parent_id)
        {
            //查找父节点信息
            Model.article_category model = GetModel(conn, trans, parent_id);
            if (model != null)
            {
                //查找子节点
                List<Model.article_category> category = new List<Model.article_category>();
                category = WriteDataBase.Query<Model.article_category>(conn, trans, Sql.Builder.Select("id").From(TableName).Where("parent_id="+ parent_id)).ToList();
                foreach (var dr in category)
                {
                    //修改子节点的ID列表及深度
                    int id = dr.id;
                    string class_list = model.class_list + id + ",";
                    int class_layer = model.class_layer + 1;
                    WriteDataBase.Execute(conn, trans, "update " + databaseprefix + "article_category set class_list='" + class_list + "', class_layer=" + class_layer + " where id=" + id); //带事务

                    //调用自身迭代
                    this.UpdateChilds(conn, trans, id); //带事务
                }
            }
        }

        /// <summary>
        /// 取得所有类别列表
        /// </summary>
        /// <returns></returns>
        public DataSet GetList()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from " + databaseprefix + "article_category");
            strSql.Append(" order by sort_id asc,id desc");
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 取得所有类别列表
        /// </summary>
        /// <param name="parent_id">父ID</param>
        /// <param name="channel_id">频道ID</param>
        /// <returns></returns>
        public DataTable GetList(int parent_id, int channel_id)
        {
            return GetList(parent_id, channel_id, "");
        }

        public DataTable GetList(int parent_id, int channel_id, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from " + databaseprefix + "article_category");
            strSql.Append(" where channel_id=" + channel_id);
            if (strWhere != "")
            {
                strSql.Append(" and " + strWhere);
            }
            strSql.Append(" order by sort_id asc,id desc");
            DataSet ds = ReadDataBase.QueryFillDataSet(strSql.ToString());
            DataTable oldData = ds.Tables[0] as DataTable;
            if (oldData == null)
            {
                return null;
            }
            //复制结构
            DataTable newData = oldData.Clone();
            //调用迭代组合成DAGATABLE
            GetChilds(oldData, newData, parent_id, channel_id);
            return newData;
        }
        /// <summary>
        /// 从内存中取得所有下级类别列表（自身迭代）
        /// </summary>
        private void GetChilds(DataTable oldData, DataTable newData, int parent_id, int channel_id)
        {
            DataRow[] dr = oldData.Select("parent_id=" + parent_id);
            for (int i = 0; i < dr.Length; i++)
            {
                DataRow row = newData.NewRow();//创建新行
                //循环查找列数量赋值
                for (int j = 0; j < dr[i].Table.Columns.Count; j++)
                {
                    row[dr[i].Table.Columns[j].ColumnName] = dr[i][dr[i].Table.Columns[j].ColumnName];
                }
                newData.Rows.Add(row);
                //调用自身迭代
                this.GetChilds(oldData, newData, int.Parse(dr[i]["id"].ToString()), channel_id);
            }
        }
        #endregion

        #region 私有方法================================
        /// <summary>
        /// 将对象转换为实体
        /// </summary>
        public Model.article_category DataRowToModel(DataRow row)
        {
            Model.article_category model = new Model.article_category();
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

        #region 统计数量
        /// <summary>
        /// 类别统计数量
        /// </summary>
        /// <param name="channel_id">频道ID</param>
        /// <param name="category_id">类别ID</param>
        /// <returns></returns>
        public int GetCount(string channel_name, int category_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name + " s join " + databaseprefix + "article_category a on a.id =s.category_id where a.class_list like '%," + category_id + ",%' and s.status=0");
            object obj = ReadDataBase.ExecuteScalar<object>(strSql.ToString());
            if (null != obj)
            {
                return Convert.ToInt32(obj);
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}