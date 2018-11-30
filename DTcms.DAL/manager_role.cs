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
    /// 数据访问类:管理角色
    /// </summary>
    public partial class manager_role : DapperRepository<Model.manager_role>
    {
        private string databaseprefix; //数据库表名前缀
        public manager_role(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 删除一条数据，及子表所有相关数据
        /// </summary>
        public bool Delete(int id)
        {
            List<string> sqllist = new List<string>();
            //删除管理角色权限
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "manager_role_value ");
            strSql.Append(" where role_id=@0");
            sqllist.Add(strSql.ToString());
            //删除管理角色
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("delete from " + databaseprefix + "manager_role ");
            strSql2.Append(" where id=@0");
            sqllist.Add(strSql2.ToString());

            int rowsAffected = WriteDataBase.ExecuteSqlTran(sqllist, id);
            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 返回角色名称
        /// </summary>
        public string GetTitle(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 role_name from " + databaseprefix + "manager_role");
            strSql.Append(" where id=" + id);
            string title = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
            if (string.IsNullOrEmpty(title))
            {
                return string.Empty;
            }
            return title;
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.manager_role model)
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
                        strSql.Append("update " + databaseprefix + "manager_role set ");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键和LIST<T>则追加sql字符串
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

                        #region 角色权限表信息====================
                        //删除角色所有的权限
                        StringBuilder strSql1 = new StringBuilder();
                        strSql1.Append("delete from " + databaseprefix + "manager_role_value where role_id=@0");
                        WriteDataBase.Execute(strSql1.ToString(), model.id);

                        //重新添加角色权限
                        if (model.manager_role_values != null)
                        {
                            
                            StringBuilder strSql2; //SQL字符串
                            StringBuilder str21; //数据库字段
                            StringBuilder str22; //声明参数
                            foreach (Model.manager_role_value modelt in model.manager_role_values)
                            {
                                i = 0;
                                strSql2 = new StringBuilder();
                                str21 = new StringBuilder();
                                str22 = new StringBuilder();
                                PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                List<object> paras2 = new List<object>();
                                strSql2.Append("insert into " + databaseprefix + "manager_role_value(");
                                foreach (PropertyInfo pi in pros2)
                                {
                                    if (!pi.Name.Equals("id"))
                                    {
                                        if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                        {
                                            str21.Append(pi.Name + ",");
                                            str22.Append("@" + i + ",");
                                            i++;
                                            if (pi.Name.Equals("role_id"))
                                            {
                                                paras2.Add(model.id);//将刚角色的ID赋值
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

                        trans.Commit(); //提交事务
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback(); //回滚事务
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.manager_role model)
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
                        strSql.Append("insert into " + databaseprefix + "manager_role(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键和LIST<T>则追加sql字符串
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

                        #region 角色权限表信息====================
                        if (model.manager_role_values != null)
                        {
                            
                            StringBuilder strSql2; //SQL字符串
                            StringBuilder str21; //数据库字段
                            StringBuilder str22; //声明参数
                            foreach (Model.manager_role_value modelt in model.manager_role_values)
                            {
                                i = 0;
                                strSql2 = new StringBuilder();
                                str21 = new StringBuilder();
                                str22 = new StringBuilder();
                                PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                List<object> paras2 = new List<object>();
                                strSql2.Append("insert into " + databaseprefix + "manager_role_value(");
                                foreach (PropertyInfo pi in pros2)
                                {
                                    if (!pi.Name.Equals("id"))
                                    {
                                        if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                        {
                                            str21.Append(pi.Name + ",");
                                            str22.Append("@" + i + ",");
                                            i++;
                                            if (pi.Name.Equals("role_id"))
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
                                WriteDataBase.Execute(conn, trans, strSql2.ToString(), paras2.ToArray());
                            }
                        }
                        #endregion

                        trans.Commit(); //提交事务
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback(); //回滚事务
                        return 0;
                    }
                }
            }
            return model.id;
        }
        #endregion
    }
}