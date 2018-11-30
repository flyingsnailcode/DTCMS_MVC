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
    /// OAuth授权用户信息
    /// </summary>
    public partial class user_oauth : DapperRepository<Model.user_oauth>
    {
        private string databaseprefix; //数据库表名前缀
        public user_oauth(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }

        #region 扩展方法================================
        /// <summary>
        /// 根据开放平台和openid返回一个实体
        /// </summary>
        public Model.user_oauth GetModel(string oauth_name, string oauth_openid)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.user_oauth model = new Model.user_oauth();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "user_oauth");
            strSql.Append(" where oauth_name=@0 and oauth_openid=@1");

            return ReadDataBase.Query<Model.user_oauth>(strSql.ToString(), oauth_name, oauth_openid).FirstOrDefault();
        }
        #endregion
    }
}