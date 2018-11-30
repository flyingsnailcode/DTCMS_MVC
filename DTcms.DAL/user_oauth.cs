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
    /// OAuth��Ȩ�û���Ϣ
    /// </summary>
    public partial class user_oauth : DapperRepository<Model.user_oauth>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public user_oauth(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }

        #region ��չ����================================
        /// <summary>
        /// ���ݿ���ƽ̨��openid����һ��ʵ��
        /// </summary>
        public Model.user_oauth GetModel(string oauth_name, string oauth_openid)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.user_oauth model = new Model.user_oauth();
            //���÷��������Ե����й�������
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//ƴ���ֶ�
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "user_oauth");
            strSql.Append(" where oauth_name=@0 and oauth_openid=@1");

            return ReadDataBase.Query<Model.user_oauth>(strSql.ToString(), oauth_name, oauth_openid).FirstOrDefault();
        }
        #endregion
    }
}