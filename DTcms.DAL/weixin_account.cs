using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;

namespace DTcms.DAL
{
	/// <summary>
	/// 数据访问类:公众平台账户
	/// </summary>
	public class weixin_account : DapperRepository<Model.weixin_account>
    {
        private string databaseprefix; //数据库表名前缀
        public weixin_account(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            List<string> sqllist = new List<string>();
            //删除微信请求回复内容表
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("delete " + databaseprefix + "weixin_request_content");
            strSql1.Append(" where account_id=@0");
            sqllist.Add(strSql1.ToString());

            //删除微信请求回复规格表
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("delete " + databaseprefix + "weixin_request_rule");
            strSql2.Append(" where account_id=@0");
            sqllist.Add(strSql2.ToString());

            //删除微信公众平台回复信息表
            StringBuilder strSql3 = new StringBuilder();
            strSql3.Append("delete from " + databaseprefix + "weixin_response_content");
            strSql3.Append(" where account_id=@0");
            sqllist.Add(strSql3.ToString());

            //删除微信ACCESS TOKEN存储表
            StringBuilder strSql4 = new StringBuilder();
            strSql4.Append("delete from " + databaseprefix + "weixin_access_token");
            strSql4.Append(" where account_id=@0");
            sqllist.Add(strSql4.ToString());

            //删除微信公众号主表
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "weixin_account");
            strSql.Append(" where id=@0");
            sqllist.Add(strSql.ToString());

            return WriteDataBase.ExecuteSqlTran(sqllist, id) > 0;
        }

        /// <summary>
        /// 公众账户和原始ID是否对应
        /// </summary>
        public bool ExistsOriginalId(int id, string originalid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "weixin_account");
            strSql.Append(" where id=@0 and originalid=@1");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), id, originalid) > 0;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public string GetToken(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 token from " + databaseprefix + "weixin_account");
            strSql.Append(" where id=@0");
            object obj = ReadDataBase.ExecuteScalar<object>(strSql.ToString(), id);
            if (obj != null)
            {
                return obj.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 返回账户名称
        /// </summary>
        public string GetName(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 name from " + databaseprefix + "weixin_account");
            strSql.Append(" where id=" + id);
            string title = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
            if (string.IsNullOrEmpty(title))
            {
                return string.Empty;
            }
            return title;
        }
        #endregion
    }
}

