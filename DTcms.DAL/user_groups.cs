using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Linq;

namespace DTcms.DAL
{
    /// <summary>
    ///  ���ݷ�����:�û����
    /// </summary>
    public partial class user_groups : DapperRepository<Model.user_groups>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public user_groups(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region ��չ����================================
        /// <summary>
        /// ��ȡ��Ա���ۿ�
        /// </summary>
        public int GetDiscount(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 discount from " + databaseprefix + "user_groups");
            strSql.Append(" where id=" + id);
            return WriteDataBase.Execute(strSql.ToString());
        }

        /// <summary>
        /// ȡ��Ĭ�����ʵ��
        /// </summary>
        public Model.user_groups GetDefault()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 *");
            strSql.Append(" from " + databaseprefix + "user_groups");
            strSql.Append(" where is_lock=0 order by is_default desc,id asc");

            return ReadDataBase.Query<Model.user_groups>(strSql.ToString()).FirstOrDefault();
        }
        /// <summary>
        /// ���ر�������
        /// </summary>
        public string GetTitle(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 title from " + databaseprefix + "user_groups");
            strSql.Append(" where id=@0");
            string title = ReadDataBase.ExecuteScalar<string>(strSql.ToString(), id);
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }
            return string.Empty;
        }

        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(int id)
        {
            //ɾ����Ա��۸�
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("delete from " + databaseprefix + "user_group_price ");
            strSql1.Append(" where group_id=@0 ");
            WriteDataBase.Execute(strSql1.ToString(), id);

            //ɾ������
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "user_groups ");
            strSql.Append(" where id=@0");

            int rowsAffected = WriteDataBase.Execute(strSql.ToString(), id);
            return rowsAffected > 0;
        }
        #endregion

        /// <summary>
        /// ���ݾ���ֵ�������������ʵ��
        /// </summary>
        public Model.user_groups GetUpgrade(int group_id, int exp)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 *");
            strSql.Append(" from " + databaseprefix + "user_groups");
            strSql.Append(" where is_lock=0 and is_upgrade=1 and grade>(select grade from " + databaseprefix + "user_groups where id=" + group_id + ") and upgrade_exp<=" + exp);
            strSql.Append(" order by grade asc");

            return ReadDataBase.Query<Model.user_groups>(strSql.ToString()).FirstOrDefault();
        }
        /// <summary>
        /// ���ݳ�ֵ�������������ʵ��
        /// </summary>
        public Model.user_groups GetUpgradePrice(int group_id, decimal price)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 *");
            strSql.Append(" from " + databaseprefix + "user_groups");
            strSql.Append(" where is_lock=0 and upgrade_price>0 and is_upgrade=1 and grade>(select grade from " + databaseprefix + "user_groups where id=" + group_id + ") and upgrade_price<=" + price);
            strSql.Append(" order by upgrade_price desc,grade asc");
            return ReadDataBase.Query<Model.user_groups>(strSql.ToString()).FirstOrDefault();
        }
    }
}