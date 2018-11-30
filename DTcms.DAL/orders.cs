using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Reflection;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:订单
    /// </summary>
    public partial class orders : DapperRepository<Model.orders>
    {
        private string databaseprefix; //数据库表名前缀
        public orders(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        public bool Exists(int user_id, int article_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "orders");
            strSql.Append(" where user_id=@0 and status>=3");
            strSql.Append(" and id in (select order_id from " + databaseprefix + "order_goods where article_id=@1)");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), user_id, article_id) > 0;
        }

        /// <summary>
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(string order_no, string strValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + databaseprefix + "orders set " + strValue);
            strSql.Append(" where order_no='" + order_no + "'");
            return WriteDataBase.Execute(strSql.ToString()) > 0;
        }

        /// <summary>
        /// 增加一条数据,及其子表数据
        /// </summary>
        public int Add(Model.orders model)
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
                        strSql.Append("insert into " + databaseprefix + "orders(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键则追加sql字符串
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

                        #region 订单商品列表======================
                        if (model.order_goods != null)
                        {
                            StringBuilder strSql2; //SQL字符串
                            StringBuilder strSql3;
                            StringBuilder strSql4;
                            StringBuilder str21; //数据库字段
                            StringBuilder str22; //声明参数
                            foreach (Model.order_goods modelt in model.order_goods)
                            {
                                i = 0;
                                strSql2 = new StringBuilder();
                                str21 = new StringBuilder();
                                str22 = new StringBuilder();
                                PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                List<object> paras2 = new List<object>();
                                strSql2.Append("insert into " + databaseprefix + "order_goods(");
                                foreach (PropertyInfo pi in pros2)
                                {
                                    if (!pi.Name.Equals("id"))
                                    {
                                        if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                        {
                                            str21.Append(pi.Name + ",");
                                            str22.Append("@" + i + ",");
                                            i++;
                                            if (pi.Name.Equals("order_id"))
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

                                //扣减商品库存
                                if (modelt.goods_id > 0)
                                {
                                    strSql3 = new StringBuilder();
                                    strSql3.Append("update " + databaseprefix + "article_goods set stock_quantity=stock_quantity-@0");
                                    strSql3.Append(" where id=@1 and channel_id=@2 and article_id=@3");
                                    WriteDataBase.Execute(conn, trans, strSql3.ToString(), modelt.quantity, modelt.goods_id, modelt.channel_id, modelt.article_id);

                                    string channelName = new DAL.site_channel(databaseprefix).GetChannelName(modelt.channel_id);//查询频道的名称
                                    strSql4 = new StringBuilder();
                                    strSql4.Append("update " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channelName + " set ");
                                    strSql4.Append("stock_quantity=(select sum(stock_quantity) from " + databaseprefix + "article_goods where channel_id=@0 and article_id=@1)");
                                    strSql4.Append(" where id=@1");
                                    WriteDataBase.Execute(conn, trans, strSql4.ToString(),modelt.channel_id, modelt.article_id);
                                }
                                else
                                {
                                    string channelName = new DAL.site_channel(databaseprefix).GetChannelName(modelt.channel_id);//查询频道的名称
                                    strSql4 = new StringBuilder();
                                    strSql4.Append("update " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + channelName + " set ");
                                    strSql4.Append("stock_quantity=stock_quantity-@0 where id=@1");
                                    WriteDataBase.Execute(conn, trans, strSql4.ToString(), modelt.quantity, modelt.article_id);
                                }
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

        /// <summary>
        /// 删除一条数据，及子表所有相关数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("delete from " + databaseprefix + "order_goods ");
            strSql2.Append(" where order_id=@0 ");
            ReadDataBase.ExecuteScalar<int>(strSql2.ToString(), id);

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "orders ");
            strSql.Append(" where id=@0");

            int rowsAffected = WriteDataBase.Execute(strSql.ToString(), id);
            return rowsAffected > 0;
        }

        /// <summary>
        /// 根据订单号获取支付方式ID
        /// </summary>
        public int GetPaymentId(string order_no)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 payment_id from " + databaseprefix + "orders");
            strSql.Append(" where order_no=@0");
            object obj = ReadDataBase.ExecuteScalar<int>(strSql.ToString(), order_no);
            if (obj != null)
            {
                return Convert.ToInt32(obj);
            }
            return 0;
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.orders> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + "orders");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return GetModelList(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public override Model.orders GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.orders model = new Model.orders();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                //拼接字段，忽略List<T>
                if (!typeof(System.Collections.IList).IsAssignableFrom(p.PropertyType))
                {
                    str1.Append(p.Name + ",");//拼接字段
                }
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "orders");
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
        /// 根据订单号返回一个实体
        /// </summary>
        public Model.orders GetModel(string order_no)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.orders model = new Model.orders();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                //拼接字段，忽略List<T>
                if (!typeof(System.Collections.IList).IsAssignableFrom(p.PropertyType))
                {
                    str1.Append(p.Name + ",");//拼接字段
                }
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(',') + " from " + databaseprefix + "orders");
            strSql.Append(" where order_no='"+ order_no + "'");

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
        /// 将对象转换为实体
        /// </summary>
        public Model.orders DataRowToModel(DataRow row)
        {
            Model.orders model = new Model.orders();
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

                #region 子表信息======================
                StringBuilder strSql1 = new StringBuilder();
                strSql1.Append("select * from " + databaseprefix + "order_goods");
                strSql1.Append(" where order_id="+ model.id);

                DataTable dt1 = ReadDataBase.QueryFillDataSet(strSql1.ToString()).Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    int rowsCount = dt1.Rows.Count;
                    List<Model.order_goods> models = new List<Model.order_goods>();
                    Model.order_goods modelt;
                    for (int n = 0; n < rowsCount; n++)
                    {
                        modelt = new Model.order_goods();
                        Type modeltType = modelt.GetType();
                        for (int i = 0; i < dt1.Rows[n].Table.Columns.Count; i++)
                        {
                            PropertyInfo proInfo = modeltType.GetProperty(dt1.Rows[n].Table.Columns[i].ColumnName);
                            if (proInfo != null && dt1.Rows[n][i] != DBNull.Value)
                            {
                                proInfo.SetValue(modelt, dt1.Rows[n][i], null);
                            }
                        }
                        models.Add(modelt);
                    }
                    model.order_goods = models;
                }
                #endregion
            }
            return model;
        }
        #endregion
    }
}