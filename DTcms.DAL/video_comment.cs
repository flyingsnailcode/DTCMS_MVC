using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Reflection;
using System.Linq;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据库访问层
    /// </summary>
    public partial class video_comment : DapperRepository<Model.video_comment>
    {
        private string databaseprefix; //数据库表名前缀
        public video_comment(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }

        #region 基本方法
        
        #endregion

        #region 增加一条数据
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="model">Model.video_comment</param>
        /// <returns>ID</returns>
        public int Add(Model.video_comment model)
        {
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();//数据字段
                        StringBuilder str2 = new StringBuilder();//数据参数
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("insert into " + databaseprefix + "video_comment(");
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
                        
                        if (model.parent_id > 0)
                        {
                            Model.video_comment model2 = GetModel(conn, trans, model.parent_id); //带事务
                            model.class_list = model2.class_list + model.id + ",";
                            model.class_layer = model2.class_layer + 1;
                        }
                        else
                        {
                            model.class_list = "," + model.id + ",";
                            model.class_layer = 1;
                        }
                        //修改节点列表和深度
                        WriteDataBase.Execute(conn, trans, "update " + databaseprefix + "video_comment set class_list='" + model.class_list + "', class_layer=" + model.class_layer + " where id=" + model.id); //带事务
                        trans.Commit();
                    }
                    catch
                    {
            			trans.Rollback();
            			return 0;
                    }
                }
            }
            return model.id;
        }
        #endregion
        
        #region 更新一条数据
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="model">Model.video_comment</param>
        /// <returns>True or False</returns>
        public bool Update(Model.video_comment model)
        {
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //先判断选中的父节点是否被包含
                        if (IsContainNode(model.id, model.parent_id))
            			{
            				//查找旧数据
            				Model.video_comment oldModel = GetModel(model.id);
            				//查找旧父节点数据
            				string class_list = "," + model.parent_id + ",";
            				int class_layer = 1;
            				if (oldModel.parent_id > 0)
            				{
            					Model.video_comment oldParentModel = GetModel(conn, trans, oldModel.parent_id); //带事务
            					class_list = oldParentModel.class_list + model.parent_id + ",";
            					class_layer = oldParentModel.class_layer + 1;
            				}
                            //先提升选中的父节点
                            WriteDataBase.Execute(conn, trans, "update " + databaseprefix + "video_comment set parent_id=" + oldModel.parent_id + ",class_list='" + class_list + "', class_layer=" + class_layer + " where id=" + model.parent_id); //带事务
            				UpdateChilds(conn, trans, model.parent_id); //带事务
            			}
            			//更新子节点
            			if (model.parent_id > 0)
            			{
            				Model.video_comment model2 = GetModel(conn, trans, model.parent_id); //带事务
            				model.class_list = model2.class_list + model.id + ",";
            				model.class_layer = model2.class_layer + 1;
            			}
            			else
            			{
            				model.class_list = "," + model.id + ",";
            				model.class_layer = 1;
            			}
                        #region 修改主表数据==========================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("update  " + databaseprefix + "article_category set ");
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
                        strSql.Append(" where id=@" + i + " ");
                        paras.Add(model.id);
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());
                        //更新子节点
                        UpdateChilds(conn, trans, model.id);
                        #endregion

            			trans.Commit();
                    }
                    catch (Exception ex)
                    {
            			trans.Rollback();
            			return false;
            			throw ex;
                    }
                }
            }
            return true;
        }
        #endregion
        
        #region 返回一个实体
        /// <summary>
        /// 得到一个对象实体(重载，带事务)
        /// </summary>
        /// <param name="conn">SQL连接</param>
        /// <param name="trans">T-SQL事务</param>
        /// <param name="id">ID号</param>
        public Model.video_comment GetModel(IDbConnection conn, IDbTransaction trans, int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 * from " + databaseprefix + "video_comment ");
            strSql.Append(" where id=@0");
            return ReadDataBase.Query<Model.video_comment>(strSql.ToString(), id).FirstOrDefault();
        }
        #endregion
        
        #region 取得所有列表
        /// <summary>
        /// 取得所有列表
        /// </summary>
        /// <param name="Top">数量</param>
        /// <param name="parent_id">父类ID</param>
        /// <param name="strWhere">条件</param>
        /// <param name="filedOrder">排序</param>
        /// <returns>DataTable</returns>
        public DataTable GetList(int Top, int parent_id, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString()+" ");
            }
            strSql.Append(" * from " + databaseprefix + "video_comment");
            if ("" != strWhere.Trim())
            {
                strSql.Append(" where " + strWhere);
            }
            if ("" != filedOrder.Trim())
            {
                strSql.Append(" order by " + filedOrder);
            }
            DataSet ds = ReadDataBase.QueryFillDataSet(strSql.ToString());
            DataTable oldData = ds.Tables[0] as DataTable;
            if (null == oldData)
            {
                return null;
            }
            //复制结构
            DataTable newData = oldData.Clone();
            //调用迭代组合成DataTable
            GetChilds(oldData, newData, parent_id);
            return newData;
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 从内存中取得所有下级类别列表（自身迭代）
        /// </summary>
        /// <param name="parent_id">父类ID</param>
        private void GetChilds(DataTable oldData, DataTable newData, int parent_id)
        {
            DataRow[] dr = oldData.Select("parent_id=" + parent_id);
            for (int i = 0; i < dr.Length; i++)
            {
                //添加一行数据
                DataRow row = newData.NewRow();
            	row["id"] = int.Parse(dr[i]["id"].ToString());
            	row["channel_id"] = int.Parse(dr[i]["channel_id"].ToString());
            	row["article_id"] = int.Parse(dr[i]["article_id"].ToString());
            	row["parent_id"] = int.Parse(dr[i]["parent_id"].ToString());
            	row["class_list"] = dr[i]["class_list"].ToString();
            	row["class_layer"] = int.Parse(dr[i]["class_layer"].ToString());
            	row["user_id"] = int.Parse(dr[i]["user_id"].ToString());
            	row["user_name"] = dr[i]["user_name"].ToString();
            	row["user_ip"] = dr[i]["user_ip"].ToString();
            	row["content"] = dr[i]["content"].ToString();
            	row["is_lock"] = int.Parse(dr[i]["is_lock"].ToString());
            	row["add_time"] = DateTime.Parse(dr[i]["add_time"].ToString());
            	row["is_reply"] = int.Parse(dr[i]["is_reply"].ToString());
            	row["reply_content"] = dr[i]["reply_content"].ToString();
            	row["reply_time"] = DateTime.Parse(dr[i]["reply_time"].ToString());
            	row["star"] = int.Parse(dr[i]["star"].ToString());
            	row["time"] = dr[i]["time"].ToString();
            	row["color"] = dr[i]["color"].ToString();
            	row["type"] = dr[i]["type"].ToString();
                newData.Rows.Add(row);
                //调用自身迭代
                this.GetChilds(oldData, newData, int.Parse(dr[i]["id"].ToString()));
            }
        }

        /// <summary>
        /// 修改子节点的ID列表及深度（自身迭代）
        /// </summary>
        /// <param name="parent_id">父类ID</param>
        private void UpdateChilds(IDbConnection conn, IDbTransaction trans, int parent_id)
        {
            //查找父节点信息
            Model.video_comment model = GetModel(conn, trans, parent_id);
            if (null != model)
            {
        		//查找子节点
        		string strSql = "select id from " + databaseprefix + "video_comment where parent_id=" + parent_id;
        		DataSet ds = ReadDataBase.QueryFillDataSet(conn, trans, strSql); //带事务
        		foreach (DataRow dr in ds.Tables[0].Rows)
        		{
        			//修改子节点的ID列表及深度
        			int id = int.Parse(dr["id"].ToString());
        			string class_list = model.class_list + id + ",";
        			int class_layer = model.class_layer + 1;
                    WriteDataBase.Execute(conn, trans, "update [" + databaseprefix + "video_comment] set class_list='" + class_list + "', class_layer=" + class_layer + " where id=" + id); //带事务
        			//调用自身迭代
        			this.UpdateChilds(conn, trans, id); //带事务
        		}
            }
        }

        /// <summary>
        /// 验证节点是否被包含
        /// </summary>
        /// <param name="id">ID号</param>
        /// <param name="parent_id">父类ID</param>
        /// <returns></returns>
        private bool IsContainNode(int id, int parent_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "video_comment where class_list like '%," + id + ",%' and id=" + parent_id);
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString()) > 0;
        }
        #endregion
    }
}
