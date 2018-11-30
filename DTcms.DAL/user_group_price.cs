using System;
using System.Collections.Generic;
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
	/// ���ݷ�����:��Ա�۸�
	/// </summary>
    public partial class user_group_price : DapperRepository<Model.user_group_price>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public user_group_price(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region ��չ����================================
        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public Model.user_group_price GetModel(int goods_id, int group_id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.user_group_price model = new Model.user_group_price();
            //���÷��������Ե����й�������
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//ƴ���ֶ�
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "user_group_price");
            strSql.Append(" where goods_id=@0 and group_id=@1");

            return ReadDataBase.Query<Model.user_group_price>(strSql.ToString(), goods_id, group_id).FirstOrDefault();
        }
        #endregion
    }
}

