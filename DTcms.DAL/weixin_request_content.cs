using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;

namespace DTcms.DAL
{
	/// <summary>
    /// ���ݷ�����:����ظ����
	/// </summary>
	public class weixin_request_content : DapperRepository<Model.weixin_request_content>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public weixin_request_content(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region ��չ����================================
        /// <summary>
        /// ���ع���ĵ�һ������
        /// </summary>
        public string GetContent(int rule_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 content from " + databaseprefix + "weixin_request_content");
            strSql.Append(" where rule_id=@0");
            object obj = ReadDataBase.ExecuteScalar<string>(strSql.ToString(), rule_id);
            if (obj != null)
            {
                return obj.ToString();
            }
            return string.Empty;
        }

        // <summary>
        /// ��ù�������б�
        /// </summary>
        public DataSet GetList(int Top, int ruleId, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,account_id,rule_id,title,content,link_url,img_url,media_id,media_url,meida_hd_url,sort_id,add_time");
            strSql.Append(" FROM " + databaseprefix + "weixin_request_content where rule_id=" + ruleId);
            if (strWhere.Trim() != "")
            {
                strSql.Append(" and " + strWhere);
            }
            strSql.Append(" order by sort_id asc,id desc");
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// ���ع����������������
        /// </summary>
        public int GetCount(int rule_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) as H ");
            strSql.Append(" from " + databaseprefix + "weixin_request_content");
            strSql.Append(" where rule_id=@0");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), rule_id);
        }

        /// <summary>
        /// ���ع���ĵ�һ������
        /// </summary>
        public string GetTitle(int rule_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 title from " + databaseprefix + "weixin_request_content");
            strSql.Append(" where rule_id=@0");
            string title = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }
            return string.Empty;
        }
        #endregion
    }
}

