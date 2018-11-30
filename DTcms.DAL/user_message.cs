using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Collections.Generic;

namespace DTcms.DAL
{
    /// <summary>
    /// �û�����Ϣ
    /// </summary>
    public partial class user_message : DapperRepository<Model.user_message>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public user_message(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region ��չ����================================
        /// <summary>
        /// ��ò�ѯ��ҳ����
        /// </summary>
        public List<Model.user_message> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + "user_message");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return GetModelList(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// �����û���ɾ��һ������
        /// </summary>
        public bool Delete(int id, string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "user_message ");
            strSql.Append(" where id=@0 and (post_user_name=@1 or accept_user_name=@2)");

            int rows = WriteDataBase.Execute(strSql.ToString(), id, user_name, user_name);
            return rows > 0;
        }
        #endregion
    }
}