using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Reflection;
using System.Linq;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:文章内容
    /// </summary>
    public class article : DapperRepository<Model.article>
    {
        private string databaseprefix; //数据库表名前缀
        public article(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string channel_name, int article_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where id=@0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), article_id) >0;
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string channel_name, string call_index)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where call_index=@0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), call_index) > 0;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.article model)
        {
            //查询频道名称
            string channelName = new DAL.site_channel(databaseprefix).GetChannelName(model.channel_id);
            if (channelName.Length == 0)
            {
                return 0;
            }
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
                        strSql.Append("insert into " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channelName + "(");
                        //主表字段信息
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键或List<T>则追加sql字符串
                            if (!pi.Name.Equals("id") && !pi.Name.Equals("fields") && !typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
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
                        //扩展字段信息
                        foreach (KeyValuePair<string, string> kvp in model.fields)
                        {
                            str1.Append(kvp.Key + ",");//拼接字段
                            str2.Append("@" + i + ",");//声明参数
                            i++;
                            paras.Add(kvp.Value);//对参数赋值
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(") values (");
                        strSql.Append(str2.ToString().Trim(','));
                        strSql.Append(") ");
                        strSql.Append(";SELECT @@@IDENTITY;");
                        object obj = WriteDataBase.ExecuteScalar<object>(conn, trans, strSql.ToString(), paras.ToArray());
                        model.id = Convert.ToInt32(obj);
                        #endregion

                        #region 添加图片相册====================
                        if (model.albums != null)
                        {
                            new DAL.article_albums(databaseprefix).Add(conn, trans, model.albums, model.channel_id, model.id);
                        }
                        #endregion

                        #region 添加内容附件====================
                        if (model.attach != null)
                        {
                            new DAL.article_attach(databaseprefix).Add(conn, trans, model.attach, model.channel_id, model.id);
                        }
                        #endregion

                        #region 添加商品价格====================
                        if (model.goods != null)
                        {
                            foreach (Model.article_goods modelt in model.goods)
                            {
                                new DAL.article_goods(databaseprefix).Add(conn, trans, modelt, model.channel_id, model.id);
                            }
                        }
                        #endregion

                        #region 添加商品规格====================
                        if (model.specs != null)
                        {
                            foreach (Model.article_goods_spec modelt in model.specs)
                            {
                                new DAL.article_goods_spec(databaseprefix).Add(conn, trans, modelt, model.channel_id, model.id);
                            }
                        }
                        #endregion

                        #region 保存自定义参数====================
                        if (model.attribute != null)
                        {
                            new DAL.article_attribute(databaseprefix).Add(conn, trans, model.attribute, model.channel_id, model.id);
                        }
                        #endregion

                        #region 添加Tags标签====================
                        if (model.tags != null && model.tags.Trim().Length > 0)
                        {
                            string[] tagsArr = model.tags.Trim().Split(',');
                            if (tagsArr.Length > 0)
                            {
                                foreach (string tagsStr in tagsArr)
                                {
                                    new DAL.article_tags(databaseprefix).Update(conn, trans, tagsStr, model.channel_id, model.id);
                                }
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
        /// 返回商品库存数量
        /// </summary>
        public int GetStockQuantity(string channel_name, int channel_id, int article_id, int goods_id)
        {
            if (goods_id > 0)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select top 1 stock_quantity ");
                strSql.Append(" from " + databaseprefix + "article_goods");
                strSql.Append(" where channel_id=" + channel_id + " and article_id=" + article_id + " and id=" + goods_id);
                return ReadDataBase.ExecuteScalar<int>(strSql.ToString());
            }
            else
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select top 1 stock_quantity ");
                strSql.Append(" from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
                strSql.Append(" where id=" + article_id);
                return ReadDataBase.ExecuteScalar<int>(strSql.ToString());
            }
        }

        /// <summary>
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(string channel_name, int id, string strValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name + " set " + strValue);
            strSql.Append(" where id=" + id);
            return WriteDataBase.Execute(strSql.ToString()) > 0;
        }
        
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.article GetModel(string channel_name, int article_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 * from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where id="+ article_id);
            DataSet ds = ReadDataBase.QueryFillDataSet(strSql.ToString());
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
        /// 删除一条数据
        /// </summary>
        public bool Delete(string channel_name, int channel_id, int article_id)
        {
            //取得相册MODEL
            List<Model.article_albums> albumsList = new DAL.article_albums(databaseprefix).GetList(channel_id, article_id);
            //取得附件MODEL
            List<Model.article_attach> attachList = new DAL.article_attach(databaseprefix).GetList(channel_id, article_id);

            //删除图片相册
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("delete from " + databaseprefix + "article_albums");
            strSql2.Append(" where channel_id=@0 and article_id=@1");
            WriteDataBase.Execute(strSql2.ToString(), channel_id, article_id);

            //删除附件
            StringBuilder strSql3 = new StringBuilder();
            strSql3.Append("delete from " + databaseprefix + "article_attach");
            strSql3.Append(" where channel_id=@0 and article_id=@1");
            WriteDataBase.Execute(strSql3.ToString(), channel_id, article_id);

            //删除用户组价格
            StringBuilder strSql4 = new StringBuilder();
            strSql4.Append("delete from " + databaseprefix + "user_group_price");
            strSql4.Append(" where channel_id=@0 and article_id=@1");
            WriteDataBase.Execute(strSql4.ToString(), channel_id, article_id);

            //删除商品价格
            StringBuilder strSql5 = new StringBuilder();
            strSql5.Append("delete from " + databaseprefix + "article_goods");
            strSql5.Append(" where channel_id=@0 and article_id=@1");
            WriteDataBase.Execute(strSql5.ToString(), channel_id, article_id);

            //删除商品规格
            StringBuilder strSql6 = new StringBuilder();
            strSql6.Append("delete from " + databaseprefix + "article_goods_spec");
            strSql6.Append(" where channel_id=@0 and article_id=@1");
            WriteDataBase.Execute(strSql6.ToString(), channel_id, article_id);

            //删除Tags标签关系
            StringBuilder strSql7 = new StringBuilder();
            strSql7.Append("delete from " + databaseprefix + "article_tags_relation");
            strSql7.Append(" where channel_id=@0 and article_id=@1");
            WriteDataBase.Execute(strSql7.ToString(), channel_id, article_id);

            //删除评论
            StringBuilder strSql8 = new StringBuilder();
            strSql8.Append("delete from " + databaseprefix + "article_comment");
            strSql8.Append(" where channel_id=@0 and article_id=@1 ");
            WriteDataBase.Execute(strSql8.ToString(), channel_id, article_id);

            //删除主表
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where id=@0");
            int rowsAffected = WriteDataBase.Execute(strSql.ToString(), article_id);
            if (rowsAffected > 0)
            {
                new DAL.article_albums(databaseprefix).DeleteFile(albumsList); //删除图片
                new DAL.article_attach(databaseprefix).DeleteFile(attachList); //删除附件
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 返回信息内容
        /// </summary>
        public string GetContent(string channel_name, int article_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 content from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where id=" + article_id);
            string content = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }
            return content;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.article GetModel(string channel_name, string call_index)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 * from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where call_index='"+ call_index + "'");

            DataSet ds = ReadDataBase.QueryFillDataSet(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }

            //Model.article model = ReadDataBase.Query<Model.article>(strSql.ToString(), call_index).FirstOrDefault();
        }

        private Model.article DataRowToModel(DataRow row)
        {
            Model.article model = new Model.article();//主表字段
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
                        //用索引值设置属性值
                        proInfo.SetValue(model, row[i], null);
                    }
                }
                #endregion

                #region 扩展字段信息===================
                Dictionary<string, string> fieldDic = new DAL.article_attribute_field(databaseprefix).GetFields(model.channel_id);//扩展字段字典
                for (int i = 0; i < row.Table.Columns.Count; i++)
                {
                    if (fieldDic.ContainsKey(row.Table.Columns[i].ColumnName) && row[i] != null)
                    {
                        fieldDic[row.Table.Columns[i].ColumnName] = row[i].ToString();
                    }
                }
                model.fields = fieldDic;
                #endregion

                //相册信息
                model.albums = new DAL.article_albums(databaseprefix).GetList(model.channel_id, model.id);
                //附件信息
                model.attach = new DAL.article_attach(databaseprefix).GetList(model.channel_id, model.id);
                //商品价格
                model.goods = new DAL.article_goods(databaseprefix).GetList(model.channel_id, model.id);
                //扩展参数信息
                model.attribute = new article_attribute(databaseprefix).GetList(model.channel_id, model.id);
            }
            return model;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.article model)
        {
            //查询频道名称
            string channelName = new DAL.site_channel(databaseprefix).GetChannelName(model.channel_id);
            if (channelName.Length == 0)
            {
                return false;
            }
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 修改主表数据==========================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("update  " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channelName + " set ");
                        //主表字段信息
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键或List<T>则追加sql字符串
                            if (!pi.Name.Equals("id") && !pi.Name.Equals("fields") && !typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
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
                        //扩展字段信息
                        foreach (KeyValuePair<string, string> kvp in model.fields)
                        {
                            str1.Append(kvp.Key + "=@" + i + ",");//声明参数
                            i++;
                            paras.Add(kvp.Value);//对参数赋值
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(" where id="+ model.id);
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());
                        #endregion

                        #region 修改图片相册==========================
                        //删除/添加/修改相册图片
                        new DAL.article_albums(databaseprefix).Update(conn, trans, model.albums, model.channel_id, model.id);
                        #endregion

                        #region 修改内容附件==========================
                        //删除/添加/修改附件
                        new DAL.article_attach(databaseprefix).Update(conn, trans, model.attach, model.channel_id, model.id);
                        #endregion

                        #region 修改商品价格==========================
                        //删除旧商品价格
                        new DAL.article_goods(databaseprefix).Delete(conn, trans, model.channel_id, model.id);
                        //添加商品价格
                        if (model.goods != null)
                        {
                            foreach (Model.article_goods modelt in model.goods)
                            {
                                new DAL.article_goods(databaseprefix).Add(conn, trans, modelt, model.channel_id, model.id);
                            }
                        }
                        #endregion

                        #region 修改商品规格==========================
                        //先删除旧的规格
                        new DAL.article_goods_spec(databaseprefix).Delete(conn, trans, model.channel_id, model.id);
                        //添加商品规格
                        if (model.specs != null)
                        {
                            foreach (Model.article_goods_spec modelt in model.specs)
                            {
                                new DAL.article_goods_spec(databaseprefix).Add(conn, trans, modelt, model.channel_id, model.id);
                            }
                        }
                        #endregion

                        #region 修改Tags标签==========================
                        //删除已有的Tags标签关系
                        new DAL.article_tags(databaseprefix).Delete(conn, trans, model.channel_id, model.id);
                        //添加添加标签
                        if (model.tags != null && model.tags.Trim().Length > 0)
                        {
                            string[] tagsArr = model.tags.Trim().Split(',');
                            if (tagsArr.Length > 0)
                            {
                                foreach (string tagsStr in tagsArr)
                                {
                                    new DAL.article_tags(databaseprefix).Update(conn, trans, tagsStr, model.channel_id, model.id);
                                }
                            }
                        }
                        #endregion

                        #region 修改扩展参数==========================
                        //删除/添加/修改扩展参数
                        new DAL.article_attribute(databaseprefix).Update(conn, trans, model.attribute, model.channel_id, model.id);
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
        #endregion

        #region 分页、取数据
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(string channel_name, int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return WriteDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 根据频道名称显示前几条数据
        /// </summary>
        public DataSet ArticleList(string channel_name, int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * FROM " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where datediff(d,add_time,getdate())>=0");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" and " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 根据频道名称显示前几条数据
        /// </summary>
        public DataSet ArticleList(string channel_name, int category_id, int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * FROM " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where datediff(d,add_time,getdate())>=0");
            if (category_id > 0)
            {
                strSql.Append(" and category_id in(select id from " + databaseprefix + "article_category where class_list like '%," + category_id + ",%')");
            }
            if (strWhere.Trim() != "")
            {
                strSql.Append(" and " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        /// <param name="pageSize">分页数量</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="strWhere">条件</param>
        /// <param name="filedOrder">排序</param>
        /// <param name="recordCount">返回数据总数</param>
        /// <returns>DataTable</returns>
        public DataSet ArticleList(string channel_name, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            if ("" != strWhere.Trim())
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return ReadDataBase.QueryFillDataSet(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 根据频道名称及规格查询分页数据
        /// </summary>
        public DataSet ArticleList(string channel_name, int category_id, Dictionary<string, string> dicSpecIds, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            string specWhere = string.Empty;
            foreach (KeyValuePair<string, string> kv in dicSpecIds)
            {
                if (Utils.StrToInt(kv.Value, 0) > 0)
                {
                    if (!string.IsNullOrEmpty(specWhere))
                    {
                        specWhere += "and ";
                    }
                    specWhere += "B.spec_ids like '%," + kv.Value + ",%'";
                }
            }
            if (!string.IsNullOrEmpty(specWhere))
            {
                specWhere = " and (" + specWhere + ")";
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where datediff(d,add_time,getdate())>=0");
            if (category_id > 0)
            {
                strSql.Append(" and category_id in(select id from " + databaseprefix + "article_category where class_list like '%," + category_id + ",%')");
            }
            if (strWhere.Trim() != "")
            {
                strSql.Append(" and " + strWhere);
            }
            if (!string.IsNullOrEmpty(specWhere))
            {
                strSql.Append(" and id in(");
                strSql.Append("select A.id from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name + " as A," + databaseprefix + "article_goods as B");
                strSql.Append(" where A.channel_id=B.channel_id and A.id=B.article_id " + specWhere);
                strSql.Append(" group by A.id)");
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return ReadDataBase.QueryFillDataSet(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 获得关健字查询分页数据(搜索用到)
        /// </summary>
        public DataSet ArticleSearch(int site_id, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            //查询站点频道列表
            DataTable dt = new DAL.site_channel(databaseprefix).GetList("site_id=" + site_id).Tables[0];
            if (dt.Rows.Count > 0)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from (");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strSql.Append("select id,site_id,channel_id,call_index,title,zhaiyao,add_time,img_url");
                    strSql.Append(" from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dt.Rows[i]["name"].ToString());
                    strSql.Append(" where datediff(d,add_time,getdate())>=0");
                    if (strWhere.Trim() != "")
                    {
                        strSql.Append(" and " + strWhere);
                    }
                    if (i < (dt.Rows.Count - 1))
                    {
                        strSql.Append(" UNION ALL ");//合并频道数据表
                    }
                }
                strSql.Append(") as temp_article");
                recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
                return ReadDataBase.QueryFillDataSet(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            recordCount = 0;
            return new DataSet();
        }

        /// <summary>
        /// 根据频道名称获得查询分页数据
        /// </summary>
        public DataSet ArticleList(string channel_name, int category_id, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where datediff(d,add_time,getdate())>=0");
            if (category_id > 0)
            {
                strSql.Append(" and category_id in(select id from " + databaseprefix + "article_category where class_list like '%," + category_id + ",%')");
            }
            if (strWhere.Trim() != "")
            {
                strSql.Append(" and " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return ReadDataBase.QueryFillDataSet(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 获得Tags查询分页数据(搜索用到)
        /// </summary>
        public DataSet ArticleSearch(int site_id, int tags, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            //查询站点频道列表
            DataTable dt = new DAL.site_channel(databaseprefix).GetList("site_id=" + site_id).Tables[0];
            if (dt.Rows.Count > 0)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from (");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strSql.Append("select id,site_id,A.channel_id,call_index,title,zhaiyao,add_time,img_url,click,category_id");
                    strSql.Append(" from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dt.Rows[i]["name"].ToString() + " as A INNER JOIN (");
                    strSql.Append("select R.channel_id,R.article_id");
                    strSql.Append(" from "+ databaseprefix + "article_tags_relation as R INNER JOIN " + databaseprefix + "article_tags as S ON R.tag_id=S.id and S.id='" + tags + "'");
                    strSql.Append(") as T ON A.channel_id=T.channel_id and A.id=T.article_id");
                    strSql.Append(" where datediff(d,add_time,getdate())>=0");
                    if (strWhere.Trim() != "")
                    {
                        strSql.Append(" and " + strWhere);
                    }
                    if (i < (dt.Rows.Count - 1))
                    {
                        strSql.Append(" UNION ALL ");//合并频道数据表
                    }
                }
                strSql.Append(") as temp_article");
                recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
                return ReadDataBase.QueryFillDataSet(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            recordCount = 0;
            return new DataSet();
        }

        /// <summary>
        /// 获得Tags查询分页数据(搜索用到)
        /// </summary>
        public DataSet ArticleSearch(string channel_name, string tags, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,channel_id,call_index,title,zhaiyao as remark,tags,add_time,click,img_url,user_name,category_id,update_time from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where status=0 and datediff(d,add_time,getdate())>=0 and id in(select article_id from " + databaseprefix + "article_tags_relation ");
            strSql.Append(" where tag_id=(select id from " + databaseprefix + "article_tags where title='" + tags + "'))");
            if (!string.IsNullOrEmpty(channel_name))
            {
                strSql.Append(" and channel_id=(select id from " + databaseprefix + "site_channel where [name]='" + channel_name + "')");
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return ReadDataBase.QueryFillDataSet(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(string channel_name, int category_id, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            if (category_id > 0)
            {
                strSql.Append(" where category_id in(select id from " + databaseprefix + "article_category where class_list like '%," + category_id + ",%')");
            }
            if (strWhere.Trim() != "")
            {
                if (category_id > 0)
                {
                    strSql.Append(" and " + strWhere);
                }
                else
                {
                    strSql.Append(" where " + strWhere);
                }
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return ReadDataBase.QueryFillDataSet(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.article> GetListPage(string channel_name, int category_id, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            if (category_id > 0)
            {
                strSql.Append(" where category_id in(select id from " + databaseprefix + "article_category where class_list like '%," + category_id + ",%')");
            }
            if (strWhere.Trim() != "")
            {
                if (category_id > 0)
                {
                    strSql.Append(" and " + strWhere);
                }
                else
                {
                    strSql.Append(" where " + strWhere);
                }
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return GetModelList(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }
        #endregion

        #region 前台模板调用方法========================
        /// <summary>
        /// 根据频道名称获取总记录数
        /// </summary>
        public int ArticleCount(string channel_name, int category_id, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channel_name);
            strSql.Append(" where datediff(d,add_time,getdate())>=0");
            if (category_id > 0)
            {
                strSql.Append(" and category_id in(select id from " + databaseprefix + "article_category where class_list like '%," + category_id + ",%')");
            }
            if (strWhere.Trim() != "")
            {
                strSql.Append(" and " + strWhere);
            }
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString());
        }
        #endregion
    }
}

