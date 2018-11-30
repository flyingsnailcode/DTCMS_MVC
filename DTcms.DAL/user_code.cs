using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Reflection;
using System.Linq;

namespace DTcms.DAL
{
    /// <summary>
    /// 用户生成码
    /// </summary>
    public partial class user_code : DapperRepository<Model.user_code>
    {
        private string databaseprefix; //数据库表名前缀
        public user_code(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string type, string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "user_code");
            strSql.Append(" where status=0 and datediff(d,eff_time,getdate())<=0 and type=@0 and user_name=@1");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), type, user_name) > 0;
        }

        /// <summary>
        /// 根据用户名得到一个对象实体
        /// </summary>
        public override Model.user_code GetModel(string user_name, string code_type, string datepart)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.user_code model = new Model.user_code();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "user_code");
            strSql.Append(" where status=0 and datediff(" + datepart + ",eff_time,getdate())<=0 and user_name=@0 and type=@1");
            return ReadDataBase.Query<Model.user_code>(strSql.ToString(), user_name, code_type).FirstOrDefault();
        }

        /// <summary>
        /// 根据生成码得到一个对象实体
        /// </summary>
        public Model.user_code GetModel(string str_code)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.user_code model = new Model.user_code();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "user_code");
            strSql.Append(" where status=0 and datediff(d,eff_time,getdate())<=0 and str_code=@0");
            return ReadDataBase.Query<Model.user_code>(strSql.ToString(), str_code).FirstOrDefault();
        }
        #endregion
    }
}