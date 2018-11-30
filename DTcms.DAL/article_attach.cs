using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Reflection;
using Dapper;
using System.Linq;

namespace DTcms.DAL
{
    /// <summary>
    /// 下载附件
    /// </summary>
    public partial class article_attach : DapperRepository<Model.article_attach>
    {
        private string databaseprefix; //数据库表名前缀
        public article_attach(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Model.article_attach> GetList(int channel_id, int article_id)
        {
            List<Model.article_attach> modelList = new List<Model.article_attach>();
            StringBuilder strSql = new StringBuilder();
            modelList = GetModelList(0, "channel_id=" + channel_id + " and article_id=" + article_id, "", "");
            return modelList;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(IDbConnection conn, IDbTransaction trans, List<Model.article_attach> models, int channel_id, int article_id)
        {
            int i = 0;
            if (models != null)
            {
                StringBuilder strSql;
                StringBuilder str1; ;//数据字段
                StringBuilder str2;//数据参数
                foreach (Model.article_attach modelt in models)
                {
                    i = 0;
                    strSql = new StringBuilder();
                    str1 = new StringBuilder();
                    str2 = new StringBuilder();
                    //利用反射获得属性的所有公共属性
                    PropertyInfo[] pros = modelt.GetType().GetProperties();
                    List<object> paras = new List<object>();
                    strSql.Append("insert into " + databaseprefix + "article_attach(");
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
        public void Update(IDbConnection conn, IDbTransaction trans, List<Model.article_attach> models, int channel_id, int article_id)
        {
            int i = 0;
            //删除已移除的图片
            DeleteList(conn, trans, models, channel_id, article_id);
            //添加/修改相册
            if (models != null)
            {
                StringBuilder strSql;
                StringBuilder str1;//数据字段
                StringBuilder str2;//数据参数
                foreach (Model.article_attach modelt in models)
                {
                    i = 0;
                    strSql = new StringBuilder();
                    str1 = new StringBuilder();
                    str2 = new StringBuilder();
                    //利用反射获得属性的所有公共属性
                    PropertyInfo[] pros = modelt.GetType().GetProperties();
                    List<object> paras = new List<object>();
                    if (modelt.id > 0)
                    {
                        strSql.Append("update " + databaseprefix + "article_attach set ");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键则追加sql字符串
                            if (!pi.Name.Equals("id"))
                            {
                                //判断属性值是否为空
                                if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                {
                                    str1.Append(pi.Name + "=@" + i + ",");//声明参数
                                    i++;
                                    paras.Add(pi.GetValue(modelt, null));//对参数赋值
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(" where id=@"+i+"");
                        paras.Add(modelt.id);
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());//带事务
                    }
                    else
                    {
                        i = 0;
                        strSql.Append("insert into " + databaseprefix + "article_attach(");
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
                                    paras.Add(pi.GetValue(modelt, null));//对参数赋值
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
        }

        /// <summary>
        /// 查找不存在的文件并删除已移除的附件及数据
        /// </summary>
        public void DeleteList(IDbConnection conn, IDbTransaction trans, List<Model.article_attach> models, int channel_id, int article_id)
        {
            StringBuilder idList = new StringBuilder();
            if (models != null)
            {
                foreach (Model.article_attach modelt in models)
                {
                    if (modelt.id > 0)
                    {
                        idList.Append(modelt.id + ",");
                    }
                }
            }
            string delIds = idList.ToString().TrimEnd(',');
            string strwhere = "channel_id=" + channel_id + " and article_id=" + article_id;
            if (!string.IsNullOrEmpty(delIds))
            {
                strwhere+=" and id not in(" + delIds + ")";
            }

            List<Model.article_attach> attach = new List<Model.article_attach>();
            attach = WriteDataBase.Query<Model.article_attach>(conn, trans, Sql.Builder.Select("id,file_path").From(TableName).Where(strwhere)).ToList();
            foreach (var dr in attach)
            {
                int rows = WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "article_attach where id=" + dr.id); //删除数据库
                if (rows > 0)
                {
                    FileHelper.DeleteFile(dr.file_path); //删除文件
                }
            }
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 检查用户是否下载过该附件
        /// </summary>
        public bool ExistsLog(int attach_id, int user_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "user_attach_log");
            strSql.Append(" where attach_id=@0 and user_id=@1");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), attach_id, user_id) > 0;
        }

        /// <summary>
        /// 插入一条下载附件记录
        /// </summary>
        public int AddLog(Model.user_attach_log model)
        {
            int i = 0;
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<object> paras = new List<object>();
            strSql.Append("insert into  " + databaseprefix + "dt_user_attach_log(");
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
            object obj = WriteDataBase.ExecuteScalar<object>(strSql.ToString(), paras.ToArray());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 获取单个附件下载次数
        /// </summary>
        public int GetDownNum(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 down_num from " + databaseprefix + "article_attach");
            strSql.Append(" where id=" + id);
            string str = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            return Convert.ToInt32(str);
        }

        /// <summary>
        /// 获取总下载次数
        /// </summary>
        public int GetCountNum(int channel_id, int article_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select sum(down_num) from " + databaseprefix + "article_attach");
            strSql.Append(" where channel_id=" + channel_id + " and article_id=" + article_id);
            string str = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            return Convert.ToInt32(str);
        }
        
        /// <summary>
        /// 删除附件文件
        /// </summary>
        public void DeleteFile(List<Model.article_attach> models)
        {
            if (models != null)
            {
                foreach (Model.article_attach modelt in models)
                {
                    FileHelper.DeleteFile(modelt.file_path);
                }
            }
        }
        #endregion
    }
}