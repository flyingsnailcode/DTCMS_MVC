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
	/// 数据访问类:频道站点
	/// </summary>
	public class sites: DapperRepository<Model.sites>
    {
        private string databaseprefix; //数据库表名前缀
        public sites(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 返回站点对应的导航ID
        /// </summary>
        public int GetSiteNavId(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select N.id from " + databaseprefix + "navigation as N," + databaseprefix + "sites as S");
            strSql.Append(" where 'channel_'+S.build_path=N.name and S.id=" + id);
            object obj = ReadDataBase.ExecuteScalar<object>(strSql.ToString());
            if (obj != null)
            {
                return Convert.ToInt32(obj);
            }
            return 0;
        }

        /// <summary>
        /// 更新数据不为默认
        /// </summary>
        public void UpDefault(IDbConnection conn, IDbTransaction trans)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + databaseprefix + "sites set is_default=0 where is_default=1");
            WriteDataBase.Execute(conn, trans, strSql.ToString());
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.sites model)
        {
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //判断当前是否为默认，如果是，取消其它默认数据
                        if (model.is_default > 0)
                        {
                            UpDefault(conn, trans);
                        }
                        #region 写入频道表数据==================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();//数据字段
                        StringBuilder str2 = new StringBuilder();//数据参数
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("insert into " + databaseprefix + "sites(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键和List<T>类型则追加sql字符串
                            if (!pi.Name.Equals("id") && !typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
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

                        //如果非继承网站则添加导航菜单
                        if (model.inherit_id == 0)
                        {
                            new DAL.navigation(databaseprefix).Add(conn, trans, "sys_contents", "channel_" + model.build_path, model.title, "", model.sort_id, 0, "Show");
                        }
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
        public bool Update(Model.sites model, string old_build_path)
        {
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //判断当前是否为默认，如果是，取消其它默认数据
                        if (model.is_default > 0)
                        {
                            UpDefault(conn, trans);
                        }
                        #region 修改频道表======================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("update  " + databaseprefix + "sites set ");
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
                        strSql.Append(" where id=@"+i+" ");
                        paras.Add(model.id);
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());
                        #endregion

                        //检查旧导航是否存在
                        if (new DAL.navigation(databaseprefix).GetModel(conn, trans, "channel_" + old_build_path) != null)
                        {
                            //如果非继承站点则修改导航菜单，是继承站点则删除旧导航
                            if (model.inherit_id == 0)
                            {
                                new DAL.navigation(databaseprefix).Update(conn, trans, "channel_" + old_build_path, "channel_" + model.build_path, model.title, model.sort_id, 0);
                            }
                            else
                            {
                                //第一种方法，只隐藏
                                new DAL.navigation(databaseprefix).Update(conn, trans, "channel_" + old_build_path, 1);
                            }
                        }
                        else if (model.inherit_id == 0) //没有旧菜单而需要添加新导航菜单
                        {
                            new DAL.navigation(databaseprefix).Add(conn, trans, "sys_contents", "channel_" + model.build_path, model.title, "", model.sort_id, 0, "Show");
                        }

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
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            string build_path = GetBuildPath(id);
            if (string.IsNullOrEmpty(build_path))
            {
                return false;
            }
            //取得要删除的所有导航ID
            string navIds = new navigation(databaseprefix).GetIds("channel_" + build_path);

            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //删除导航
                        if (!string.IsNullOrEmpty(navIds))
                        {
                            WriteDataBase.Execute("delete from [" + databaseprefix + "navigation] where id in(" + navIds + ")");
                        }
                        //删除站点
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("delete from " + databaseprefix + "sites ");
                        strSql.Append(" where id=@0");
                        WriteDataBase.Execute(strSql.ToString(), id);

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
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(int id, string strValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + databaseprefix + "sites set " + strValue);
            strSql.Append(" where id=" + id);
            int rows = WriteDataBase.Execute(strSql.ToString());
            return rows > 0;
        }

        /// <summary>
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(string build_path, string strValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + databaseprefix + "sites set " + strValue);
            strSql.Append(" where build_path=@0");
            int rows = WriteDataBase.Execute(strSql.ToString(), build_path);
            return rows > 0;
        }

        /// <summary>
        /// 查询生成目录名是否存在
        /// </summary>
        public bool Exists(string build_path)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "sites");
            strSql.Append(" where build_path=@0 ");

            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), build_path) > 0;
        }

        /// <summary>
        /// 返回站点名称
        /// </summary>
        public string GetTitle(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 title from " + databaseprefix + "sites");
            strSql.Append(" where id=" + id);
            string title = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }
            return string.Empty;
        }

        /// <summary>
        /// 返回站点名称
        /// </summary>
        public string GetTitle(string build_path)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 title from " + databaseprefix + "sites");
            strSql.Append(" where build_path=@0");
            string title = ReadDataBase.ExecuteScalar<string>(strSql.ToString(), build_path);
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }
            return string.Empty;
        }

        /// <summary>
        /// 返回站点的生成目录名
        /// </summary>
        public string GetBuildPath(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 build_path from " + databaseprefix + "sites");
            strSql.Append(" where id=" + id);
            object obj = ReadDataBase.ExecuteScalar<object>(strSql.ToString());
            if (obj != null)
            {
                return Convert.ToString(obj);
            }
            return string.Empty;
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
            strSql.Append("* from [" + databaseprefix + "sites]");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 将对象转换为实体
        /// </summary>
        public Model.sites DataRowToModel(DataRow row)
        {
            Model.sites model = new Model.sites();
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
        #endregion
    }
}

