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
	/// ���ݷ�����:����ƽ̨�˻�
	/// </summary>
	public class weixin_account : DapperRepository<Model.weixin_account>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public weixin_account(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region ��չ����================================
        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(int id)
        {
            List<string> sqllist = new List<string>();
            //ɾ��΢������ظ����ݱ�
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("delete " + databaseprefix + "weixin_request_content");
            strSql1.Append(" where account_id=@0");
            sqllist.Add(strSql1.ToString());

            //ɾ��΢������ظ�����
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("delete " + databaseprefix + "weixin_request_rule");
            strSql2.Append(" where account_id=@0");
            sqllist.Add(strSql2.ToString());

            //ɾ��΢�Ź���ƽ̨�ظ���Ϣ��
            StringBuilder strSql3 = new StringBuilder();
            strSql3.Append("delete from " + databaseprefix + "weixin_response_content");
            strSql3.Append(" where account_id=@0");
            sqllist.Add(strSql3.ToString());

            //ɾ��΢��ACCESS TOKEN�洢��
            StringBuilder strSql4 = new StringBuilder();
            strSql4.Append("delete from " + databaseprefix + "weixin_access_token");
            strSql4.Append(" where account_id=@0");
            sqllist.Add(strSql4.ToString());

            //ɾ��΢�Ź��ں�����
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "weixin_account");
            strSql.Append(" where id=@0");
            sqllist.Add(strSql.ToString());

            return WriteDataBase.ExecuteSqlTran(sqllist, id) > 0;
        }

        /// <summary>
        /// �����˻���ԭʼID�Ƿ��Ӧ
        /// </summary>
        public bool ExistsOriginalId(int id, string originalid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "weixin_account");
            strSql.Append(" where id=@0 and originalid=@1");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), id, originalid) > 0;
        }

        /// <summary>
        /// �õ�һ������ʵ��
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
        /// �����˻�����
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

