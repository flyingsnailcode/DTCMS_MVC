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
    /// ���ݷ�����:�˻��洢AccessToKenֵ
	/// </summary>
	public class weixin_access_token : DapperRepository<Model.weixin_access_token>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public weixin_access_token(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region ��չ����================================
        /// <summary>
        /// �Ƿ���ڸù����˻���¼
        /// </summary>
        public bool ExistsAccount(int account_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "weixin_access_token");
            strSql.Append(" where account_id=@0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), account_id) > 0;
        }

        /// <summary>
        /// ��ȡ�ù����˻���AccessTokenʵ��
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

