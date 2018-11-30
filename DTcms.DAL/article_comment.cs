using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:内容评论
    /// </summary>
    public partial class article_comment : DapperRepository<Model.article_comment>
    {
        private string databaseprefix; //数据库表名前缀
        public article_comment(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 查找关联用户的评理管理
        /// </summary>
        public List<Model.article_comment> GetList(string channelName, int channelId, int user_id, bool relation, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + "article_comment");
            strSql.Append(" where datediff(d,add_time,getdate())>=0 and id>0");
            if (channelId > 0)
            {
                strSql.Append(" and channel_id=" + channelId);
            }
            if (relation)
            {
                if (!string.IsNullOrEmpty(channelName))
                {
                    strSql.Append(" and article_id=(select id from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channelName + " where [user_id]=" + user_id + ")");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(channelName))
                {
                    strSql.Append(" and user_id=" + user_id);
                }
            }

            if ("" != strWhere.Trim())
            {
                strSql.Append(" and " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return GetModelList(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + "article_comment");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return ReadDataBase.QueryFillDataSet(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.article_comment model)
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
                        strSql.Append("insert into  " + databaseprefix + "article_comment(");
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
                        if (model.parent_id > 0)
                        {
                            Model.article_comment model2 = GetModel(conn, trans, model.parent_id); //带事务
                            model.class_list = model2.class_list + model.id + ",";
                            model.class_layer = model2.class_layer + 1;
                        }
                        else
                        {
                            model.class_list = "," + model.id + ",";
                            model.class_layer = 1;
                        }
                        //修改节点列表和深度
                        WriteDataBase.Execute(conn, trans, "update [" + databaseprefix + "article_comment] set class_list='" + model.class_list + "', class_layer=" + model.class_layer + " where id=" + model.id); //带事务
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
        /// 得到一个对象实体(重载，带事务)
        /// </summary>
        /// <param name="conn">SQL连接</param>
        /// <param name="trans">T-SQL事务</param>
        /// <param name="id">ID号</param>
        public Model.article_comment GetModel(IDbConnection conn, IDbTransaction trans, int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from " + databaseprefix + "article_comment where id=@0");
            return ReadDataBase.Query<Model.article_comment>(conn, trans, strSql.ToString(), id).FirstOrDefault();
        }
        #endregion
    }
}