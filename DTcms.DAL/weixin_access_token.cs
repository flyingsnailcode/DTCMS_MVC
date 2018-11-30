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
    /// 数据访问类:账户存储AccessToKen值
	/// </summary>
	public class weixin_access_token : DapperRepository<Model.weixin_access_token>
    {
        private string databaseprefix; //数据库表名前缀
        public weixin_access_token(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 是否存在该公众账户记录
        /// </summary>
        public bool ExistsAccount(int account_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "weixin_access_token");
            strSql.Append(" where account_id=@0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), account_id) > 0;
        }

        /// <summary>
        /// 获取该公众账户的AccessToken实体
        /// </summary>
        public Model.weixin_access_token GetAccountModel(int account_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 * from " + databaseprefix + "weixin_access_token");
            strSql.Append(" where account_id=@0");
            return ReadDataBase.Query<Model.weixin_access_token>(strSql.ToString(), account_id).FirstOrDefault();
        }
        #endregion
    }
}

