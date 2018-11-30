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
	/// 图片相册
	/// </summary>
	public partial class article_albums : DapperRepository<Model.article_albums>
    {
        private string databaseprefix; //数据库表名前缀
        public article_albums(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataTable GetImagesList(int article_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,article_id,thumb_path,original_path,sort_id,remark,add_time,channel_id ");
            strSql.Append(" FROM " + databaseprefix + "article_albums ");
            strSql.Append(" where article_id=" + article_id);
            strSql.Append(" order by sort_id asc, id asc");
            DataSet ds = WriteDataBase.QueryFillDataSet(strSql.ToString());
            return ds.Tables[0];
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(IDbConnection conn, IDbTransaction trans, List<Model.article_albums> models, int channel_id, int article_id)
        {
            int i = 0;
            if (models != null)
            {
                StringBuilder strSql;
                StringBuilder str1; ;//数据字段
                StringBuilder str2;//数据参数
                foreach (Model.article_albums modelt in models)
                {
                    i = 0;
                    strSql = new StringBuilder();
                    str1 = new StringBuilder();
                    str2 = new StringBuilder();
                    //利用反射获得属性的所有公共属性
                    PropertyInfo[] pros = modelt.GetType().GetProperties();
                    List<object> paras = new List<object>();
                    strSql.Append("insert into " + databaseprefix + "article_albums(");
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
        public void Update(IDbConnection conn, IDbTransaction trans, List<Model.article_albums> models, int channel_id, int article_id)
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
                foreach (Model.article_albums modelt in models)
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
                        strSql.Append("update " + databaseprefix + "article_albums set ");
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
                        strSql.Append("insert into " + databaseprefix + "article_albums(");
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
        /// 查找不存在的图片并删除已移除的图片及数据
        /// </summary>
        public void DeleteList(IDbConnection conn, IDbTransaction trans, List<Model.article_albums> models, int channel_id, int article_id)
        {
            StringBuilder idList = new StringBuilder();
            if (models != null)
            {
                foreach (Model.article_albums modelt in models)
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
                strwhere += " and id not in(" + delIds + ")";
            }
            List<Model.article_albums> albums = new List<Model.article_albums>();
            albums = WriteDataBase.Query<Model.article_albums>(conn, trans, Sql.Builder.Select("channel_id,id,thumb_path,original_path").From(TableName).Where(strwhere)).ToList();
            foreach (var dr in albums)
            {
                int rows = WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "article_albums where id=" + dr.id); //删除数据库
                if (rows > 0)
                {
                    FileHelper.DeleteFile(dr.thumb_path); //删除缩略图
                    FileHelper.DeleteFile(dr.original_path); //删除原图
                }
            }
        }

        /// <summary>
        /// 删除相册图片
        /// </summary>
        public void DeleteFile(List<Model.article_albums> models)
        {
            if (models != null)
            {
                foreach (Model.article_albums modelt in models)
                {
                    FileHelper.DeleteFile(modelt.thumb_path);
                    FileHelper.DeleteFile(modelt.original_path);
                }
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Model.article_albums> GetList(int channel_id, int article_id)
        {
            List<Model.article_albums> modelList = new List<Model.article_albums>();
            modelList = GetModelList(0, "channel_id=" + channel_id + " and article_id=" + article_id, "", "");
            return modelList;
        }
        #endregion

    }
}

