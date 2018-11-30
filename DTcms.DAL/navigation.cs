using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Linq;

namespace DTcms.DAL
{
	/// <summary>
	/// 数据访问类:后台导航菜单
	/// </summary>
	public partial class navigation : DapperRepository<Model.navigation>
    {
        private string databaseprefix; //数据库表名前缀
        public navigation(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 根据导航的名称查询其ID
        /// </summary>
        public int GetNavId(string nav_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 id from " + databaseprefix + "navigation");
            strSql.Append(" where name=@0");
            string str = ReadDataBase.ExecuteScalar<string>(strSql.ToString(), nav_name);
            return Utils.StrToInt(str, 0);
        }

        /// <summary>
        /// 查询是否存在该记录
        /// </summary>
        public bool Exists(string name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "navigation");
            strSql.Append(" where name=@0 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), name) > 0;
        }

        /// <summary>
        /// 更新图标目录
        /// </summary>
        /// <param name="oldName">原目录</param>
        /// <param name="newName">新目录</param>
        public void updateicon(string oldName, string newName)
        {
            WriteDataBase.Execute("update [" + databaseprefix + "navigation] set icon_url = replace(icon_url,'/" + oldName + "/','/" + newName + "/')");
        }

        /// <summary>
        /// 删除一条数据，带事务
        /// </summary>
        public bool Delete(IDbConnection conn, IDbTransaction trans, string nav_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "navigation");
            strSql.Append(" where name=@0");
            return WriteDataBase.Execute(conn, trans, strSql.ToString(), nav_name) > 0;
        }

        /// <summary>
        /// 得到一个对象实体，带事务
        /// </summary>
        public Model.navigation GetModel(IDbConnection conn, IDbTransaction trans, string nav_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 *");
            strSql.Append(" from " + databaseprefix + "navigation ");
            strSql.Append(" where name=@0");
            return ReadDataBase.Query<Model.navigation>(conn, trans, strSql.ToString(), nav_name).FirstOrDefault();
        }

        #region 增、改
        /// <summary>
        /// 返回ID
        /// </summary>
        public int GetId(IDbConnection conn, IDbTransaction trans, string name)
        {
            object obj = ReadDataBase.ExecuteScalar<object>("select id from " + databaseprefix + "navigation where name=@0", name);
            if (!string.IsNullOrEmpty(obj.ToString()))
            {
                return Convert.ToInt32(obj);
            }
            return 0;
        }

        /// <summary>
        /// 修改一条记录，带事务
        /// </summary>
        public bool Update(IDbConnection conn, IDbTransaction trans, string nav_name, int is_lock)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + databaseprefix + "navigation set is_lock=@0");
            strSql.Append(" where name=@1");
            int rows = WriteDataBase.Execute(conn, trans, strSql.ToString(), is_lock, nav_name);
            return rows > 0;
        }

        /// <summary>
        /// 修改一条记录，带事务
        /// </summary>
        public bool Update(IDbConnection conn, IDbTransaction trans, string old_name, string new_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + databaseprefix + "navigation set name=@0");
            strSql.Append(" where name=@1");
            int rows = WriteDataBase.Execute(conn, trans, strSql.ToString(), new_name, old_name);
            return rows > 0;
        }

        /// <summary>
        /// 修改一条记录，带事务
        /// </summary>
        public bool Update(IDbConnection conn, IDbTransaction trans, string old_name, string new_name, string title, int sort_id, int is_lock)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + databaseprefix + "navigation set");
            strSql.Append(" name=@0,");
            strSql.Append(" title=@1,");
            strSql.Append(" sort_id=@2,");
            strSql.Append(" is_lock=@3");
            strSql.Append(" where name=@4");
            int rows = WriteDataBase.Execute(conn, trans, strSql.ToString(), new_name, title, sort_id, is_lock, old_name);
            return rows > 0;
        }

        /// <summary>
        /// 快捷添加系统默认导航
        /// </summary>
        public int Add(string parent_name, string nav_name, string title, string link_url, int sort_id, int channel_id, string action_type)
        {
            //先根据名称查询该父ID
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("select top 1 id from " + databaseprefix + "navigation");
            strSql1.Append(" where name=@0");
            object obj1 = ReadDataBase.ExecuteScalar<object>(strSql1.ToString(), parent_name);
            if (obj1 == null)
            {
                return 0;
            }
            int parent_id = Convert.ToInt32(obj1);
            
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into " + databaseprefix + "navigation(");
            strSql.Append("parent_id,channel_id,nav_type,name,title,link_url,sort_id,action_type,is_lock,is_sys)");
            strSql.Append(" values (");
            strSql.Append("@0,@1,@2,@3,@4,@5,@6,@7,@8,@9)");
            strSql.Append(";SELECT @@@IDENTITY;");
            object obj2 = WriteDataBase.ExecuteScalar<object>(strSql.ToString(), parent_id, channel_id, DTEnums.NavigationEnum.System.ToString(), nav_name, title, link_url, sort_id, action_type, 0, 1);
            return Convert.ToInt32(obj2);
        }

        /// <summary>
        /// 快捷添加系统默认导航，带事务
        /// </summary>
        public int Add(IDbConnection conn, IDbTransaction trans, int parent_id, string nav_name, string title, string link_url, int sort_id, int channel_id, string action_type)
        {
            return Add(conn, trans, parent_id, nav_name, title, link_url, sort_id, channel_id, action_type, 0);
        }

        /// <summary>
        /// 修改一条记录，带事务
        /// </summary>
        public bool Update(IDbConnection conn, IDbTransaction trans, string old_name, int parent_id, string nav_name, string title, int sort_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + databaseprefix + "navigation set");
            strSql.Append(" parent_id=@0,");
            strSql.Append(" name=@1,");
            strSql.Append(" title=@2,");
            strSql.Append(" sort_id=@3");
            strSql.Append(" where name=@4");
            int rows = WriteDataBase.Execute(conn, trans, strSql.ToString(), parent_id, nav_name, title, sort_id, old_name);
            return rows > 0;
        }


        /// <summary>
        /// 快捷添加系统默认导航，带事务
        /// </summary>
        public int Add(IDbConnection conn, IDbTransaction trans, string parent_name, string nav_name, string title, string link_url, int sort_id, int channel_id, string action_type)
        {
            //先根据名称查询该父ID
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 id from " + databaseprefix + "navigation");
            strSql.Append(" where name=@0");
            object obj = ReadDataBase.ExecuteScalar<object>(strSql.ToString(), parent_name);
            if (obj == null)
            {
                return 0;
            }
            int parent_id = Convert.ToInt32(obj);

            return Add(conn, trans, parent_id, nav_name, title, link_url, sort_id, channel_id, action_type, 0);
        }

        /// <summary>
        /// 快捷添加系统默认导航，带事务
        /// </summary>
        public int Add(IDbConnection conn, IDbTransaction trans, string parent_name, string nav_name, string title, string link_url, int sort_id, int channel_id, string action_type, int is_lock)
        {
            //先根据名称查询该父ID
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 id from " + databaseprefix + "navigation");
            strSql.Append(" where name=@0");
            object obj = ReadDataBase.ExecuteScalar<object>(strSql.ToString(), parent_name);
            if (obj == null)
            {
                return 0;
            }
            int parent_id = Convert.ToInt32(obj);

            return Add(conn, trans, parent_id, nav_name, title, link_url, sort_id, channel_id, action_type, is_lock);
        }

        /// <summary>
        /// 快捷添加系统默认导航，带事务
        /// </summary>
        public int Add(IDbConnection conn, IDbTransaction trans, int parent_id, string nav_name, string title, string link_url, int sort_id, int channel_id, string action_type, int is_lock)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into " + databaseprefix + "navigation(");
            strSql.Append("parent_id,channel_id,nav_type,name,title,link_url,sort_id,action_type,is_sys,is_lock)");
            strSql.Append(" values (");
            strSql.Append("@0,@1,@2,@3,@4,@5,@6,@7,@8,@9)");
            strSql.Append(";SELECT @@@IDENTITY;");
            object obj = WriteDataBase.ExecuteScalar<object>(conn, trans, strSql.ToString(), parent_id, channel_id, DTEnums.NavigationEnum.System.ToString(),nav_name, title, link_url, sort_id, action_type, 1, is_lock);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "navigation");
            strSql.Append(" where id in(" + GetIds(id) + ")");

            int rows = WriteDataBase.Execute(strSql.ToString());
            return rows > 0;
        }
        #endregion
        /// <summary>
        /// 获取父类下的所有子类ID(含自己本身)
        /// </summary>
        public string GetIds(int parent_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,parent_id");
            strSql.Append(" FROM " + databaseprefix + "navigation");
            DataSet ds = ReadDataBase.QueryFillDataSet(strSql.ToString());
            string ids = parent_id.ToString() + ",";
            GetChildIds(ds.Tables[0], parent_id, ref ids);
            return ids.TrimEnd(',');
        }

        /// <summary>
        /// 获取父类下的所有子类ID(含自己本身)
        /// </summary>
        public string GetIds(string nav_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 id from " + databaseprefix + "navigation");
            strSql.Append(" where name=@0");
            object obj = ReadDataBase.ExecuteScalar<string>(strSql.ToString(), nav_name);
            if (obj != null)
            {
                return GetIds(Convert.ToInt32(obj));
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取类别列表
        /// </summary>
        /// <param name="parent_id">父ID</param>
        /// <param name="nav_type">导航类别</param>
        /// <returns>DataTable</returns>
        public DataTable GetList(int parent_id, string nav_type)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,parent_id,channel_id,nav_type,name,title,sub_title,icon_url,link_url,sort_id,is_lock,remark,action_type,is_sys");
            strSql.Append(" FROM " + databaseprefix + "navigation");
            strSql.Append(" where nav_type='" + nav_type + "'");
            strSql.Append(" order by sort_id asc,id desc");
            DataSet ds = ReadDataBase.QueryFillDataSet(strSql.ToString());
            //重组列表
            DataTable oldData = ds.Tables[0] as DataTable;
            if (oldData == null)
            {
                return null;
            }
            //创建一个新的DataTable增加一个深度字段
            DataTable newData = new DataTable();
            newData.Columns.Add("id", typeof(int));
            newData.Columns.Add("parent_id", typeof(int));
            newData.Columns.Add("channel_id", typeof(int));
            newData.Columns.Add("class_layer", typeof(int));
            newData.Columns.Add("nav_type", typeof(string));
            newData.Columns.Add("name", typeof(string));
            newData.Columns.Add("title", typeof(string));
            newData.Columns.Add("sub_title", typeof(string));
            newData.Columns.Add("icon_url", typeof(string));
            newData.Columns.Add("link_url", typeof(string));
            newData.Columns.Add("sort_id", typeof(int));
            newData.Columns.Add("is_lock", typeof(int));
            newData.Columns.Add("remark", typeof(string));
            newData.Columns.Add("action_type", typeof(string));
            newData.Columns.Add("is_sys", typeof(int));
            //调用迭代组合成DAGATABLE
            GetChilds(oldData, newData, parent_id, 0);
            return newData;
        }
        /// <summary>
        /// 从内存中取得所有下级类别列表（自身迭代）
        /// </summary>
        private void GetChilds(DataTable oldData, DataTable newData, int parent_id, int class_layer)
        {
            class_layer++;
            DataRow[] dr = oldData.Select("parent_id=" + parent_id);
            for (int i = 0; i < dr.Length; i++)
            {
                //添加一行数据
                DataRow row = newData.NewRow();
                row["id"] = int.Parse(dr[i]["id"].ToString());
                row["parent_id"] = int.Parse(dr[i]["parent_id"].ToString());
                row["channel_id"] = int.Parse(dr[i]["channel_id"].ToString());
                row["class_layer"] = class_layer;
                row["nav_type"] = dr[i]["nav_type"].ToString();
                row["name"] = dr[i]["name"].ToString();
                row["title"] = dr[i]["title"].ToString();
                row["sub_title"] = dr[i]["sub_title"].ToString();
                row["icon_url"] = dr[i]["icon_url"].ToString();
                row["link_url"] = dr[i]["link_url"].ToString();
                row["sort_id"] = int.Parse(dr[i]["sort_id"].ToString());
                row["is_lock"] = int.Parse(dr[i]["is_lock"].ToString());
                row["remark"] = dr[i]["remark"].ToString();
                row["action_type"] = dr[i]["action_type"].ToString();
                row["is_sys"] = int.Parse(dr[i]["is_sys"].ToString());
                newData.Rows.Add(row);
                //调用自身迭代
                this.GetChilds(oldData, newData, int.Parse(dr[i]["id"].ToString()), class_layer);
            }
        }

        /// <summary>
        /// 获取父类下的所有子类ID
        /// </summary>
        private void GetChildIds(DataTable dt, int parent_id, ref string ids)
        {
            DataRow[] dr = dt.Select("parent_id=" + parent_id);
            for (int i = 0; i < dr.Length; i++)
            {
                ids += dr[i]["id"].ToString() + ",";
                //调用自身迭代
                this.GetChildIds(dt, int.Parse(dr[i]["id"].ToString()), ref ids);
            }
        }
        #endregion
    }
}

