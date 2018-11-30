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
    /// 数据访问类:管理员
    /// </summary>
    public partial class manager : DapperRepository<Model.manager>
    {
        private string databaseprefix; //数据库表名前缀
        public manager(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  " + databaseprefix + "manager");
            strSql.Append(" where id=@0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), id) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int RootTotal()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "manager");
            strSql.Append(" where Purview=0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString());
        }

        /// <summary>
        /// 查询用户名是否存在
        /// </summary>
        public bool Exists(string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "manager");
            strSql.Append(" where user_name=@0 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), user_name) > 0;
        }
        /// <summary>
        /// 根据用户名取得Salt
        /// </summary>
        public string GetSalt(string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 salt from " + databaseprefix + "manager");
            strSql.Append(" where user_name=@0");
            string salt = ReadDataBase.ExecuteScalar<string>(strSql.ToString(), user_name);
            if (string.IsNullOrEmpty(salt))
            {
                return string.Empty;
            }
            return salt;
        }
        /// <summary>
        /// 根据用户名密码返回一个实体
        /// </summary>
        public Model.manager GetModel(string user_name, string password)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from " + databaseprefix + "manager");
            strSql.Append(" where user_name=@0 and password=@1 and is_lock=0");
            return ReadDataBase.Query<Model.manager>(strSql.ToString(), user_name, password).FirstOrDefault();
        }
        #endregion
    }
}