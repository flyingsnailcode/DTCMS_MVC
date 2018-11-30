using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using Dapper;
using System.Linq;
using System.Reflection;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:频道
    /// </summary>
    public class site_channel : DapperRepository<Model.site_channel>
    {
        private string databaseprefix; //数据库表名前缀
        public site_channel(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 返回网点ID
        /// </summary>
        public int GetSiteId(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select site_id from " + databaseprefix + "site_channel where id=@0 ");
            object obj = ReadDataBase.ExecuteScalar<object>(strSql.ToString(), id);
            if (null != obj)
            {
                return Convert.ToInt32(obj);
            }
            return 0;
        }

        /// <summary>
        /// 返回频道ID
        /// </summary>
        public int GetChannelId(string name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 id from " + databaseprefix + "site_channel");
            strSql.Append(" where name=@0 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), name);
        }
        /// <summary>
        /// 返回频道名称
        /// </summary>
        public string GetChannelName(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 name from " + databaseprefix + "site_channel");
            strSql.Append(" where id=" + id);
            object obj = ReadDataBase.ExecuteScalar<object>(strSql.ToString());
            if (obj != null)
            {
                return Convert.ToString(obj);
            }
            return string.Empty;
        }
        /// <summary>
        /// 返回频道标题
        /// </summary>
        public string GetChannelTitle(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 title from " + databaseprefix + "site_channel");
            strSql.Append(" where id=" + id);
            string title = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }
            return string.Empty;
        }
        /// <summary>
        /// 查询是否存在该记录
        /// </summary>
        public bool Exists(string name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "site_channel");
            strSql.Append(" where name=@0 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), name) > 0;
        }
        /// <summary>
        /// 返回频道的ID和名称列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,[name] FROM  " + databaseprefix + "site_channel");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by sort_id asc,id desc");
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 增加一条数据及其子表
        /// </summary>
        public int Add(Model.site_channel model)
        {
            int i = 0;
            //取得站点对应的导航ID
            int parent_id = new DAL.sites(databaseprefix).GetSiteNavId(model.site_id);
            if (parent_id == 0)
            {
                return 0;
            }
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 写入频道表数据==================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();//数据字段
                        StringBuilder str2 = new StringBuilder();//数据参数
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("insert into " + databaseprefix + "site_channel(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键和List<T>类型则追加sql字符串
                            if (!pi.Name.Equals("id") && !typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
                            {
                                //判断属性值是否为空
                                if (pi.GetValue(model, null) != null)
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

                        //写入扩展字段及创建频道数据表
                        FieldAdd(conn, trans, model);

                        #region 缩略图字段
                        //添加缩略图字段
                        if (model.channel_thums != null)
                        {
                            StringBuilder strSql2; //SQL字符串
                            StringBuilder str21; //数据库字段
                            StringBuilder str22; //声明参数
                            foreach (Model.site_channel_thum modelt in model.channel_thums)
                            {
                                i = 0;
                                //新增
                                strSql2 = new StringBuilder();
                                str21 = new StringBuilder();
                                str22 = new StringBuilder();
                                PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                List<object> paras2 = new List<object>();
                                strSql2.Append("insert into " + databaseprefix + "site_channel_thum(");
                                foreach (PropertyInfo pi in pros2)
                                {
                                    if (!pi.Name.Equals("id"))
                                    {
                                        if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                        {
                                            str21.Append(pi.Name + ",");
                                            str22.Append("@" + i + ",");
                                            i++;
                                            if (pi.Name.Equals("channel_id"))
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

                        #region 写入导航数据===============
                        DAL.navigation dal = new DAL.navigation(databaseprefix);
                        int newNavId = dal.Add(conn, trans, parent_id, "channel_" + model.name, model.title, "", model.sort_id, model.id, "Show");
                        dal.Add(conn, trans, newNavId, "channel_" + model.name + "_list", "内容管理", "/admin/article/article_list", 99, model.id, "Show,View,Add,Edit,Delete,Audit");
                        dal.Add(conn, trans, newNavId, "channel_" + model.name + "_category", "栏目类别", "/admin/article/category_list", 100, model.id, "Show,View,Add,Edit,Delete");
                        //开启规格新增菜单
                        if (model.is_spec > 0)
                        {
                            new DAL.navigation(databaseprefix).Add(conn, trans, newNavId, "channel_" + model.name + "_spec", "规格管理", "/admin/article/spec_list", 102, model.id, "Show,View,Add,Edit,Delete");
                        }
                        if (model.is_comment == 1)
                        {
                            dal.Add(conn, trans, newNavId, "channel_" + model.name + "_comment", "评论管理", "/admin/article/comment_list", 101, model.id, "Show,View,Delete,Reply");
                        }
                        if (model.is_recycle == 1)
                        {
                            dal.Add(conn, trans, newNavId, "channel_" + model.name + "_recycle", "回收站", "/admin/article/recycle_list", 102, model.id, "Show,View,Edit,Delete,Audit");
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
        public bool Update(Model.site_channel model)
        {
            int i = 0;
            Model.site_channel oldModel = GetModel(model.id); //旧的数据
            //取得站点对应的导航ID
            int parent_id = new DAL.sites(databaseprefix).GetSiteNavId(model.site_id);
            if (parent_id == 0)
            {
                return false;
            }
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 修改频道表======================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("update  " + databaseprefix + "site_channel set ");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键则追加sql字符串
                            //!pi.Name.Equals("channel_fields")
                            if (!pi.Name.Equals("id") && !typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
                            {
                                //判断属性值是否为空
                                if (pi.GetValue(model, null) != null)
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
                        #endregion

                        //删除已移除扩展字段及频道数据表列
                        FieldDelete(conn, trans, model, oldModel);
                        //编辑扩展字段及频道数据表
                        FieldUpdate(conn, trans, model, oldModel);
                        
                        #region 缩略图尺寸
                        //删除已移除的尺寸
                        ThumDelete(conn, trans, model.channel_thums, model.id);

                        //添加扩展字段
                        if (model.channel_thums != null)
                        {
                            StringBuilder strSql2; //SQL字符串
                            StringBuilder str21; //数据库字段
                            StringBuilder str22; //声明参数

                            foreach (Model.site_channel_thum modelt in model.channel_thums)
                            {
                                if (modelt.id > 0)
                                {
                                    i = 0;
                                    //更新
                                    strSql2 = new StringBuilder();
                                    str21 = new StringBuilder();
                                    PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                    List<object> paras2 = new List<object>();
                                    strSql2.Append("update " + databaseprefix + "site_channel_thum set ");
                                    foreach (PropertyInfo pi in pros2)
                                    {
                                        //如果不是主键则追加sql字符串
                                        if (!pi.Name.Equals("id"))
                                        {
                                            //判断属性值是否为空
                                            if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                            {
                                                str21.Append(pi.Name + "=@" + i + ",");//声明参数
                                                i++;
                                                paras2.Add(pi.GetValue(modelt, null));//对参数赋值
                                            }
                                        }
                                    }
                                    strSql2.Append(str21.ToString().Trim(','));
                                    strSql2.Append(" where id=@" + i + " ");
                                    paras2.Add(modelt.id);
                                    WriteDataBase.Execute(conn, trans, strSql2.ToString(), paras2.ToArray());
                                }
                                else
                                {
                                    i = 0;
                                    //新增
                                    strSql2 = new StringBuilder();
                                    str21 = new StringBuilder();
                                    str22 = new StringBuilder();
                                    PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                    List<object> paras2 = new List<object>();
                                    strSql2.Append("insert into " + databaseprefix + "site_channel_thum(");
                                    foreach (PropertyInfo pi in pros2)
                                    {
                                        if (!pi.Name.Equals("id"))
                                        {
                                            if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                            {
                                                str21.Append(pi.Name + ",");
                                                str22.Append("@" + i + ",");
                                                i++;
                                                if (pi.Name.Equals("channel_id"))
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
                        }
                        #endregion

                        #region
                        int newNavId = 0;
                        DAL.navigation dal = new DAL.navigation(databaseprefix);
                        if (!dal.Exists("channel_" + model.name))
                        {
                            //添加导航菜单
                            newNavId = dal.Add(conn, trans, parent_id, "channel_" + model.name, model.title, "", model.sort_id, model.id, "Show");
                            dal.Add(conn, trans, newNavId, "channel_" + model.name + "_list", "内容管理", "//admin/article/article_list", 99, model.id, "Show,View,Add,Edit,Delete,Audit");
                            dal.Add(conn, trans, newNavId, "channel_" + model.name + "_category", "栏目类别", "//admin/article/category_list", 100, model.id, "Show,View,Add,Edit,Delete");
                            //是否开启规格
                            if (model.is_spec > 0)
                            {
                                dal.Add(conn, trans, newNavId, "channel_" + model.name + "_spec", "规格管理", "//admin/article/spec_list", 101, model.id, "Show,View,Add,Edit,Delete");
                            }
                            //是否开启评论
                            if (model.is_comment > 0)
                            {
                                dal.Add(conn, trans, newNavId, "channel_" + model.name + "_comment", "评论管理", "//admin/article/comment_list", 101, model.id, "Show,View,Delete,Audit,Reply");
                            }
                            //是否开启回收站
                            if (model.is_recycle > 0)
                            {
                                dal.Add(conn, trans, newNavId, "channel_" + model.name + "_recycle", "回收站", "//admin/article/recycle_list", 102, model.id, "Show,View,Edit,Delete,Audit");
                            }
                        }
                        else
                        {
                            //修改菜单
                            newNavId = new DAL.navigation(databaseprefix).GetId(conn, trans, "channel_" + oldModel.name);
                            dal.Update(conn, trans, "channel_" + oldModel.name, parent_id, "channel_" + model.name, model.title, model.sort_id);
                            dal.Update(conn, trans, "channel_" + oldModel.name + "_list", "channel_" + model.name + "_list"); //内容管理
                            dal.Update(conn, trans, "channel_" + oldModel.name + "_category", "channel_" + model.name + "_category"); //栏目类别
                            //是否开启规格
                            if (model.is_spec > 0)
                            {
                                if (ReadDataBase.ExecuteScalar<int>(conn, trans, "select count(*) as H from [" + databaseprefix + "navigation] where name='channel_" + model.name + "_spec'") == 0)
                                {
                                    dal.Add(conn, trans, newNavId, "channel_" + model.name + "_spec", "规格管理", "//admin/article/spec_list", 101, model.id, "Show,View,Add,Edit,Delete");
                                }
                                else
                                {
                                    dal.Update(conn, trans, "channel_" + oldModel.name + "_spec", "channel_" + model.name + "_spec"); //评论管理
                                }
                            }
                            else
                            {
                                dal.Delete(conn, trans, "channel_" + model.name + "_spec");
                            }
                            //是否开启评论
                            if (model.is_comment > 0)
                            {
                                if (ReadDataBase.ExecuteScalar<int>(conn, trans, "select count(*) as H from [" + databaseprefix + "navigation] where name='channel_" + model.name + "_comment'") == 0)
                                {
                                    dal.Add(conn, trans, newNavId, "channel_" + model.name + "_comment", "评论管理", "//admin/article/comment_list", 101, model.id, "Show,View,Delete,Audit,Reply");
                                }
                                else
                                {
                                    dal.Update(conn, trans, "channel_" + oldModel.name + "_comment", "channel_" + model.name + "_comment"); //评论管理
                                }
                            }
                            else
                            {
                                dal.Delete(conn, trans, "channel_" + model.name + "_comment");
                            }
                            //是否开启回收站
                            if (model.is_recycle > 0)
                            {
                                if (ReadDataBase.ExecuteScalar<int>(conn, trans, "select count(*) as H from [" + databaseprefix + "navigation] where name='channel_" + model.name + "_recycle'") == 0)
                                {
                                    dal.Add(conn, trans, newNavId, "channel_" + model.name + "_recycle", "回收站", "article/recycle_list.aspx", 102, model.id, "Show,View,Edit,Delete,Audit");
                                }
                                else
                                {
                                    dal.Update(conn, trans, "channel_" + oldModel.name + "_recycle", "channel_" + model.name + "_recycle"); //回收站
                                }
                            }
                            else
                            {
                                dal.Delete(conn, trans, "channel_" + model.name + "_recycle");
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
        /// 返回指定扩展字段频道列表
        /// </summary>
        public DataSet GetFieldList(IDbConnection conn, IDbTransaction trans, int field_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select C.id,C.name");
            strSql.Append(" FROM " + databaseprefix + "site_channel as C INNER JOIN " + databaseprefix + "site_channel_field as F");
            strSql.Append(" on F.channel_id=C.id and F.field_id=" + field_id);
            return ReadDataBase.QueryFillDataSet(conn, trans, strSql.ToString());
        }

        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.site_channel DataRowToModel(IDbConnection conn, IDbTransaction trans, DataRow row)
        {
            Model.site_channel model = new Model.site_channel();
            if (row != null)
            {
                #region 主表信息======================
                //利用反射获得属性的所有公共属性
                Type modelType = model.GetType();
                for (int i = 0; i < row.Table.Columns.Count; i++)
                {
                    PropertyInfo proInfo = modelType.GetProperty(row.Table.Columns[i].ColumnName);
                    if (proInfo != null && row[i] != DBNull.Value)
                    {
                        proInfo.SetValue(model, row[i], null);//用索引值设置属性值
                    }
                }
                #endregion

                #region 子表信息======================
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from " + databaseprefix + "site_channel_field");
                strSql.Append(" where channel_id=" + model.id);
                DataTable dt = ReadDataBase.QueryFillDataSet(strSql.ToString()).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    int rowsCount = dt.Rows.Count;
                    List<Model.site_channel_field> models = new List<Model.site_channel_field>();
                    Model.site_channel_field modelt;
                    for (int n = 0; n < rowsCount; n++)
                    {
                        modelt = new Model.site_channel_field();
                        Type modeltType = modelt.GetType();
                        for (int i = 0; i < dt.Rows[n].Table.Columns.Count; i++)
                        {
                            PropertyInfo proInfo = modeltType.GetProperty(dt.Rows[n].Table.Columns[i].ColumnName);
                            if (proInfo != null && dt.Rows[n][i] != DBNull.Value)
                            {
                                proInfo.SetValue(modelt, dt.Rows[n][i], null);
                            }
                        }
                        models.Add(modelt);
                    }
                    model.channel_fields = models;
                }
                #endregion

                #region 缩略图尺寸====================
                StringBuilder strSql3 = new StringBuilder();
                strSql3.Append("select * from " + databaseprefix + "site_channel_thum");
                strSql3.Append(" where channel_id=" + model.id);
                DataSet ds3 = ReadDataBase.QueryFillDataSet(strSql3.ToString());

                if (ds3.Tables[0].Rows.Count > 0)
                {
                    int i = ds3.Tables[0].Rows.Count;
                    List<Model.site_channel_thum> models = new List<Model.site_channel_thum>();
                    Model.site_channel_thum modelt;
                    for (int n = 0; n < i; n++)
                    {
                        modelt = new Model.site_channel_thum();
                        Type modeltType = modelt.GetType();
                        for (int j = 0; j < dt.Rows[n].Table.Columns.Count; j++)
                        {
                            PropertyInfo proInfo = modeltType.GetProperty(dt.Rows[n].Table.Columns[j].ColumnName);
                            if (proInfo != null && dt.Rows[n][j] != DBNull.Value)
                            {
                                proInfo.SetValue(modelt, dt.Rows[n][j], null);
                            }
                        }
                        models.Add(modelt);
                    }
                    model.channel_thums = models;
                }
                #endregion
            }
            return model;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.site_channel GetModel(string channel_name)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.site_channel model = new Model.site_channel();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                //拼接字段，忽略List<T>
                if (!typeof(System.Collections.IList).IsAssignableFrom(p.PropertyType))
                {
                    str1.Append(p.Name + ",");
                }
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "site_channel");
            strSql.Append(" where name='"+ channel_name + "'");

            DataTable dt = ReadDataBase.QueryFillDataSet(strSql.ToString()).Tables[0];
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
        /// 得到一个对象实体
        /// </summary>
        public override Model.site_channel GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.site_channel model = new Model.site_channel();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                //拼接字段，忽略List<T>
                if (!typeof(System.Collections.IList).IsAssignableFrom(p.PropertyType))
                {
                    str1.Append(p.Name + ",");
                }
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "site_channel");
            strSql.Append(" where id=" + id);

            DataTable dt = ReadDataBase.QueryFillDataSet(strSql.ToString()).Tables[0];
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
        /// 得到一个对象实体，带事务
        /// </summary>
        public Model.site_channel GetModel(IDbConnection conn, IDbTransaction trans, int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 id,site_id,name,title,is_albums,is_attach,is_spec,sort_id from " + databaseprefix + "site_channel ");
            strSql.Append(" where id=@0 ");
            return ReadDataBase.Query<Model.site_channel>(conn, trans, strSql.ToString(), id).FirstOrDefault();
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
            strSql.Append("* from [" + databaseprefix + "site_channel]");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            //取得频道的名称
            string channel_name = GetChannelName(id);
            if (string.IsNullOrEmpty(channel_name))
            {
                return false;
            }
            //取得要删除的所有导航ID
            string navIds = new navigation(databaseprefix).GetIds("channel_" + channel_name);

            try
            {
                //删除导航主表
                if (!string.IsNullOrEmpty(navIds))
                {
                    WriteDataBase.Execute("delete from " + databaseprefix + "navigation where id in(" + navIds + ")");
                }

                //删除频道扩展字段表
                StringBuilder strSql2 = new StringBuilder();
                strSql2.Append("delete from " + databaseprefix + "site_channel_field ");
                strSql2.Append(" where channel_id=@0 ");
                WriteDataBase.Execute(strSql2.ToString(), id);

                //删除频道缩略图尺寸表
                StringBuilder strSql5 = new StringBuilder();
                strSql5.Append("delete from " + databaseprefix + "site_channel_thum ");
                strSql5.Append(" where channel_id=@0 ");
                WriteDataBase.Execute(strSql5.ToString(), id);

                //删除频道数据表
                StringBuilder strSql4 = new StringBuilder();
                strSql4.Append("drop table " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
                WriteDataBase.Execute(strSql4.ToString());

                //删除频道表
                StringBuilder strSql3 = new StringBuilder();
                strSql3.Append("delete from " + databaseprefix + "site_channel ");
                strSql3.Append(" where id=@0 ");
                WriteDataBase.Execute(strSql3.ToString(), id);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 新增扩展字段及创建频道数据表
        /// </summary>
        private void FieldAdd(IDbConnection conn, IDbTransaction trans, Model.site_channel model)
        {
            string fieldIds = string.Empty;//存储已加截的扩展字段ID集合
            //新增扩展字段表及存储字段的ID
            if (model.channel_fields != null)
            {
                StringBuilder strSql1;
                foreach (Model.site_channel_field modelt in model.channel_fields)
                {
                    fieldIds += modelt.field_id + ",";
                    strSql1 = new StringBuilder();
                    strSql1.Append("insert into " + databaseprefix + "site_channel_field(");
                    strSql1.Append("channel_id,field_id)");
                    strSql1.Append(" values (");
                    strSql1.Append("@0,@1)");
                    WriteDataBase.Execute(conn, trans, strSql1.ToString(), model.id, modelt.field_id);
                }
            }
            //创建频道数据表
            StringBuilder strSql2 = new StringBuilder();//存储创建频道表SQL语句
            strSql2.Append("CREATE TABLE " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + model.name + "(\r\n");
            strSql2.Append("[id] int IDENTITY(1,1) PRIMARY KEY,\r\n");
            strSql2.Append("[site_id] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[channel_id] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[category_id] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[brand_id] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[call_index] nvarchar(50),\r\n");
            strSql2.Append("[title] nvarchar(100),\r\n");
            strSql2.Append("[link_url] nvarchar(255),\r\n");
            strSql2.Append("[img_url] nvarchar(255),\r\n");
            strSql2.Append("[seo_title] nvarchar(255),\r\n");
            strSql2.Append("[seo_keywords] nvarchar(255),\r\n");
            strSql2.Append("[seo_description] nvarchar(255),\r\n");
            strSql2.Append("[tags] nvarchar(500),\r\n");
            strSql2.Append("[zhaiyao] nvarchar(255),\r\n");
            strSql2.Append("[content] ntext,\r\n");
            strSql2.Append("[sort_id] int NOT NULL DEFAULT ((99)),\r\n");
            strSql2.Append("[click] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[status] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[is_msg] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[is_top] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[is_red] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[is_hot] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[is_slide] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[is_sys] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[user_name] nvarchar(100),\r\n");
            strSql2.Append("[like_count] int NOT NULL DEFAULT ((0)),\r\n");
            strSql2.Append("[add_time] datetime NOT NULL DEFAULT (getdate()),\r\n");
            strSql2.Append("[update_time] datetime,\r\n");
            if (fieldIds.Length > 0)
            {
                //查询扩展字段表
                DataTable dt = new DAL.article_attribute_field(databaseprefix).GetList(0, "id in(" + fieldIds.Trim(',') + ")", "sort_id asc,id desc").Tables[0];
                //判断及组合创表SQL语句
                foreach (DataRow dr in dt.Rows)
                {
                    strSql2.Append("[" + dr["name"].ToString() + "] " + dr["data_type"].ToString() + ",\r\n");
                }
            }
            //执行SQL创表语句
            WriteDataBase.Execute(conn, trans, strSql2.ToString().TrimEnd(',') + ")");
        }

        /// <summary>
        /// 编辑扩展字段及频道数据表
        /// </summary>
        private void FieldUpdate(IDbConnection conn, IDbTransaction trans, Model.site_channel newModel, Model.site_channel oldModel)
        {
            if (newModel.channel_fields != null)
            {
                string newFieldIds = string.Empty; //用来存储新增的字段ID
                //添加扩展字段
                StringBuilder strSql1;
                foreach (Model.site_channel_field modelt in newModel.channel_fields)
                {
                    strSql1 = new StringBuilder();
                    Model.site_channel_field fieldModel = null;
                    if (oldModel.channel_fields != null)
                    {
                        fieldModel = oldModel.channel_fields.Find(p => p.field_id == modelt.field_id); //查找是否已经存在
                    }
                    if (fieldModel == null) //如果不存在则添加
                    {
                        newFieldIds += modelt.field_id + ","; //以逗号分隔开存储
                        strSql1.Append("insert into " + databaseprefix + "site_channel_field(");
                        strSql1.Append("channel_id,field_id)");
                        strSql1.Append(" values (");
                        strSql1.Append("@0,@1)");
                        WriteDataBase.Execute(conn, trans, strSql1.ToString(), modelt.channel_id, modelt.field_id);
                    }
                }
                //添加频道数据表列
                if (newFieldIds.Length > 0)
                {
                    List<Model.article_attribute_field> field = new List<Model.article_attribute_field>();
                    field = WriteDataBase.Query<Model.article_attribute_field>(conn, trans, Sql.Builder.Select("id,name,data_type").From(databaseprefix + "article_attribute_field").Where("id in(" + newFieldIds.TrimEnd(',') + ")")).ToList();
                    foreach (var dr in field)
                    {
                        ReadDataBase.Execute(conn, trans, "alter table " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + oldModel.name + " add " + dr.name + " " + dr.data_type);
                        //(!string.IsNullOrEmpty(dr.default_value) ? "DEFAULT "+ dr.default_value : "")
                    }
                }

            }
            //如果频道名称改变则需要更改数据表名
            if (newModel.name != oldModel.name)
            {
                ReadDataBase.Execute(conn, trans, "exec sp_rename '" + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + oldModel.name + "', '" + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + newModel.name + "'");
            }
        }

        /// <summary>
        /// 删除已移除的扩展字段及频道数据表列
        /// </summary>
        private void FieldDelete(IDbConnection conn, IDbTransaction trans, Model.site_channel newModel, Model.site_channel oldModel)
        {
            if (oldModel.channel_fields == null)
            {
                return;
            }
            string fieldIds = string.Empty;
            foreach (Model.site_channel_field modelt in oldModel.channel_fields)
            {
                //查找对应的字段ID，不在旧实体则删除
                if (newModel.channel_fields.Find(p => p.field_id == modelt.field_id) == null)
                {
                    //记住要删除的字段ID
                    fieldIds += modelt.field_id + ",";
                    //删除该旧字段
                    WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "site_channel_field where channel_id=" + newModel.id + " and field_id=" + modelt.field_id);
                }
            }
            //删除频道数据表列
            if (fieldIds.Length > 0)
            {
                List<Model.article_attribute_field> field = new List<Model.article_attribute_field>();
                field = WriteDataBase.Query<Model.article_attribute_field>(conn, trans, Sql.Builder.Select("id,name").From(databaseprefix + "article_attribute_field").Where("id in(" + fieldIds.TrimEnd(',') + ")")).ToList();
                foreach (var dr in field)
                {
                    //删除频道数据表列
                    ReadDataBase.Execute(conn, trans, "alter table " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + oldModel.name + " drop column " + dr.name);
                }
            }
        }

        /// <summary>
        /// 删除已移除的图片扩展字段
        /// </summary>
        private void ThumDelete(IDbConnection conn, IDbTransaction trans, List<Model.site_channel_thum> thums, int channel_id)
        {
            List<Model.site_channel_thum> thum = new List<Model.site_channel_thum>();
            thum = WriteDataBase.Query<Model.site_channel_thum>(conn, trans, Sql.Builder.Select("id").From(databaseprefix + "site_channel_thum").Where("channel_id=" + channel_id)).ToList();
            foreach (var dr in thum)
            {
                Model.site_channel_thum model = thums.Find(p => p.id == dr.id); //查找对应的字段ID
                if (model == null)
                {
                    WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "site_channel_thum where channel_id=" + channel_id + " and id=" + dr.id);//删除该行
                }
            }
        }

        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.site_channel DataRowToModel(DataRow row)
        {
            Model.site_channel model = new Model.site_channel();
            if (row != null)
            {
                #region 主表信息======================
                //利用反射获得属性的所有公共属性
                Type modelType = model.GetType();
                for (int i = 0; i < row.Table.Columns.Count; i++)
                {
                    PropertyInfo proInfo = modelType.GetProperty(row.Table.Columns[i].ColumnName);
                    if (proInfo != null && row[i] != DBNull.Value)
                    {
                        proInfo.SetValue(model, row[i], null);//用索引值设置属性值
                    }
                }
                #endregion

                #region 子表信息======================
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from " + databaseprefix + "site_channel_field");
                strSql.Append(" where channel_id=" + model.id);
                DataTable dt = ReadDataBase.QueryFillDataSet(strSql.ToString()).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    int rowsCount = dt.Rows.Count;
                    List<Model.site_channel_field> models = new List<Model.site_channel_field>();
                    Model.site_channel_field modelt;
                    for (int n = 0; n < rowsCount; n++)
                    {
                        modelt = new Model.site_channel_field();
                        Type modeltType = modelt.GetType();
                        for (int i = 0; i < dt.Rows[n].Table.Columns.Count; i++)
                        {
                            PropertyInfo proInfo = modeltType.GetProperty(dt.Rows[n].Table.Columns[i].ColumnName);
                            if (proInfo != null && dt.Rows[n][i] != DBNull.Value)
                            {
                                proInfo.SetValue(modelt, dt.Rows[n][i], null);
                            }
                        }
                        models.Add(modelt);
                    }
                    model.channel_fields = models;
                }
                #endregion

                #region 缩略图尺寸====================
                StringBuilder strSql3 = new StringBuilder();
                strSql3.Append("select id,title,class_id,channel_id,width,height,typeid,is_lock,add_time from " + databaseprefix + "site_channel_thum");
                strSql3.Append(" where channel_id=" + model.id);
                DataTable ds3 = ReadDataBase.QueryFillDataSet(strSql3.ToString()).Tables[0];

                if (ds3.Rows.Count > 0)
                {
                    int i = ds3.Rows.Count;
                    List<Model.site_channel_thum> models = new List<Model.site_channel_thum>();
                    Model.site_channel_thum modelt;
                    for (int n = 0; n < i; n++)
                    {
                        modelt = new Model.site_channel_thum();
                        Type modeltType = modelt.GetType();
                        for (int j = 0; j < ds3.Rows[n].Table.Columns.Count; j++)
                        {
                            PropertyInfo proInfo = modeltType.GetProperty(ds3.Rows[n].Table.Columns[j].ColumnName);
                            if (proInfo != null && ds3.Rows[n][j] != DBNull.Value)
                            {
                                proInfo.SetValue(modelt, ds3.Rows[n][j], null);
                            }
                        }
                        models.Add(modelt);
                    }
                    model.channel_thums = models;
                }
                #endregion
            }
            return model;
        }
        #endregion

        #region 私有方法

        #endregion
    }
}

