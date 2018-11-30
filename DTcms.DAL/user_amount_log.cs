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
    /// 账户金额记录
    /// </summary>
    public partial class user_amount_log : DapperRepository<Model.user_amount_log>
    {
        private string databaseprefix; //数据库表名前缀
        public user_amount_log(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.user_amount_log> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + "user_amount_log");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return GetModelList(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.user_amount_log model)
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
                        strSql.Append("insert into " + databaseprefix + "user_amount_log(");
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
                        #endregion

                        #region 用户表信息========================
                        StringBuilder strSql1 = new StringBuilder();
                        strSql1.Append("update " + databaseprefix + "users set amount=amount+" + model.value);
                        strSql1.Append(" where id=@0");
                        WriteDataBase.Execute(conn, trans, strSql1.ToString(), model.user_id);
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
        /// 增加一条数据(带事务)
        /// </summary>
        public int Add(IDbConnection conn, IDbTransaction trans, Model.user_amount_log model)
        {
            int i = 0;
            #region 主表信息==========================
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<object> paras = new List<object>();
            strSql.Append("insert into " + databaseprefix + "user_amount_log(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("id"))
                {
                    //判断属性值是否为空
                    if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                    {
                        str1.Append(pi.Name + ",");//拼接字段
                        str2.Append("@" +i + ",");//声明参数
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

            #region 用户表信息========================
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("update " + databaseprefix + "users set amount=amount+" + model.value);
            strSql1.Append(" where id=@0");
            WriteDataBase.Execute(conn, trans, strSql1.ToString(), model.user_id);
            #endregion

            return model.id;
        }

        /// <summary>
        /// 根据用户名删除一条数据
        /// </summary>
        public bool Delete(int id, string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "user_amount_log ");
            strSql.Append(" where id=@0 and user_name=@1");

            int rows = WriteDataBase.Execute(strSql.ToString(), id, user_name);
            return rows > 0;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.user_amount_log model)
        {
            int i = 0;
            #region 主表信息==========================
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<object> paras = new List<object>();
            strSql.Append("update " + databaseprefix + "user_amount_log set ");
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
            WriteDataBase.Execute(strSql.ToString(), paras.ToArray());
            #endregion

            #region 用户表信息========================
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("update " + databaseprefix + "users set amount=amount+" + model.value);
            strSql1.Append(" where id=@0");
            #endregion

            return WriteDataBase.Execute(strSql1.ToString(), model.user_id) > 0;
        }
        #endregion
    }
}