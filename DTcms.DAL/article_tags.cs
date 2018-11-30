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
    /// 数据访问类:Tag标签
    /// </summary>
    public partial class article_tags : DapperRepository<Model.article_tags>
    {
        private string databaseprefix; //数据库表名前缀
        public article_tags(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 检查更新Tags标签及关系，带事务
        /// </summary>
        public void Update(IDbConnection conn, IDbTransaction trans, string tags_title, int channel_id, int article_id)
        {
            int tagsId = 0;
            //检查该Tags标签是否已存在
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 id from " + databaseprefix + "article_tags");
            strSql.Append(" where title=@0");
            object obj1 = ReadDataBase.ExecuteScalar<object>(conn, trans, strSql.ToString(), tags_title);
            if (obj1 != null)
            {
                //存在则将ID赋值
                tagsId = Convert.ToInt32(obj1);
            }
            //如果尚未创建该Tags标签则创建
            if (tagsId == 0)
            {
                StringBuilder strSql2 = new StringBuilder();
                strSql2.Append("insert into " + databaseprefix + "article_tags(");
                strSql2.Append("title,is_red,sort_id,add_time)");
                strSql2.Append(" values (");
                strSql2.Append("@0,@1,@2,@3)");
                strSql2.Append(";SELECT @@@IDENTITY;");
                object obj2 = WriteDataBase.ExecuteScalar<object>(conn, trans, strSql2.ToString(), tags_title, 0, 99, DateTime.Now);
                if (obj2 != null)
                {
                    //插入成功后返回ID
                    tagsId = Convert.ToInt32(obj2);
                }
            }
            //匹配Tags标签与文章之间的关系
            if (tagsId > 0)
            {
                StringBuilder strSql3 = new StringBuilder();
                strSql3.Append("insert into " + databaseprefix + "article_tags_relation(");
                strSql3.Append("channel_id,article_id,tag_id)");
                strSql3.Append(" values (");
                strSql3.Append("@0,@1,@2)");
                WriteDataBase.Execute(conn, trans, strSql3.ToString(), channel_id, article_id, tagsId);
            }
        }

        /// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Model.article_spec model)
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
                                    str1.Append(pi.Name + "=@" +i + ",");
                                    i++;
                                    //对参数赋值
                                    paras.Add(pi.GetValue(model, null));
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(" where id=@"+i+" ");
                        paras.Add(model.id);
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());
                        #endregion

                        #region 规格值信息========================
                        //删除已删除的规格值
                        DeleteValues(conn, trans, model.values, model.id);
                        //添加/修改规格值
                        if (model.values != null)
                        {
                            StringBuilder strSql2; //SQL字符串
                            StringBuilder str21; //数据库字段
                            StringBuilder str22; //声明参数
                            foreach (Model.article_spec_value modelt in model.values)
                            {
                                i = 0;
                                strSql2 = new StringBuilder();
                                str21 = new StringBuilder();
                                str22 = new StringBuilder();
                                PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                List<object> paras2 = new List<object>();
                                if (modelt.id > 0)
                                {
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
                                    strSql2.Append(" where id=@"+i+" ");
                                    paras2.Add(modelt.id);
                                    WriteDataBase.Execute(conn, trans, strSql2.ToString(), paras2.ToArray());
                                }
                                else
                                {
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
                                    WriteDataBase.Execute(conn, trans, strSql2.ToString(), paras2.ToArray());
                                }
                            }
                        }
                        #endregion

                        trans.Commit();//提交事务
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();//回滚事务
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
		/// 增加一条数据
		/// </summary>
		public int Add(Model.article_spec model)
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
                        object obj = WriteDataBase.ExecuteScalar<object>(conn, trans, strSql.ToString(), paras.ToArray());
                        model.id = Convert.ToInt32(obj);
                        #endregion

                        #region 规格值信息========================
                        if (model.values != null)
                        {
                            
                            StringBuilder strSql2;//SQL字符串
                            StringBuilder str21;//数据库字段
                            StringBuilder str22;//声明参数
                            foreach (Model.article_spec_value modelt in model.values)
                            {
                                i = 0;
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
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            //删除Tag标签关系表
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("delete from " + databaseprefix + "article_tags_relation");
            strSql1.Append(" where tag_id=@0");
            int rowsAffected = WriteDataBase.Execute(strSql1.ToString(), id);

            //删除主表
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "article_tags");
            strSql.Append(" where id=@0");
            rowsAffected += WriteDataBase.Execute(strSql.ToString(), id);

            return rowsAffected > 1;
        }

        /// <summary>
        /// 删除文章对应的Tags标签关系
        /// </summary>
        public bool Delete(IDbConnection conn, IDbTransaction trans, int channel_id, int article_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "article_tags_relation");
            strSql.Append(" where channel_id=@0 and article_id=@1");
            return WriteDataBase.Execute(strSql.ToString(), channel_id, article_id) > 0;
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
            strSql.Append(" *,(select count(1) from " + databaseprefix + "article_tags_relation where tag_id=" + databaseprefix + "article_tags.id) as count");
            strSql.Append(" FROM  " + databaseprefix + "article_tags");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string call_index)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  " + databaseprefix + "article_tags");
            strSql.Append(" where ");
            strSql.Append(" call_index = @0 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), call_index) > 0;
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select *,(select count(0) from " + databaseprefix + "article_tags_relation where tag_id=" + databaseprefix + "article_tags.id) as count FROM " + databaseprefix + "article_tags");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return ReadDataBase.QueryFillDataSet(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }
        #endregion

        #region 私有方法========================
        /// <summary>
        /// 查找不存在的规格值并删除
        /// </summary>
        private void DeleteValues(IDbConnection conn, IDbTransaction trans, List<Model.article_spec_value> models, int parentId)
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
                    WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "article_spec where parent_id=" + parentId + " and id not in(" + id_list + ")");
                }
            }
        }
        /// <summary>
        /// 查找不存在的规格值并删除
        /// </summary>
        private void DeleteList(IDbConnection conn, IDbTransaction trans, List<Model.article_spec_value> models, int parentId)
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
                    WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "article_spec where parent_id=" + parentId + " and id not in(" + id_list + ")");
                }
            }
        }
        #endregion
    }
}