using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DTcms.DBUtility;
using DTcms.Common;

namespace DTcms.DAL
{
    /// <summary>
    /// ���ݷ�����:�û�
    /// </summary>
    public partial class users : DapperRepository<Model.users>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public users(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region ��չ����================================
        /// <summary>
        /// �Ƿ���ڸü�¼
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "users");
            strSql.Append(" where id=@0 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), id) > 0;
        }
        /// <summary>
        /// ����û����Ƿ����
        /// </summary>
        public bool Exists(string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "users");
            strSql.Append(" where user_name=@0 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), user_name) > 0;
        }

        /// <summary>
        /// ���ͬһIPע����(Сʱ)���Ƿ����
        /// </summary>
        public bool Exists(string reg_ip, int regctrl)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "users");
            strSql.Append(" where reg_ip=@0 and DATEDIFF(hh,reg_time,getdate())<@1 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), reg_ip, regctrl) > 0;
        }
        /// <summary>
        /// �����û������뷵��һ��ʵ��
        /// </summary>
        public Model.users GetModel(string user_name, string password, int emaillogin, int mobilelogin)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 * from [" + databaseprefix + "users] where (user_name=@0");
            if (emaillogin == 1)
            {
                strSql.Append(" or email=@0");
            }
            if (mobilelogin == 1)
            {
                strSql.Append(" or mobile=@0");
            }
            strSql.Append(") and password=@1 and status<3");
            
            return ReadDataBase.Query<Model.users>(strSql.ToString(), user_name, password).FirstOrDefault();
        }

        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(int id)
        {
            //��ȡ�û�������
            Model.users model = Get(id);
            if (model == null)
            {
                return false;
            }
            
            //ɾ�����ּ�¼
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("delete from " + databaseprefix + "user_point_log ");
            strSql1.Append(" where user_id=@0");
            WriteDataBase.Execute(strSql1.ToString(), id);

            //ɾ������¼
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("delete from " + databaseprefix + "user_amount_log ");
            strSql2.Append(" where user_id=@0");
            WriteDataBase.Execute(strSql2.ToString(), id);

            //ɾ�����������¼
            StringBuilder strSql3 = new StringBuilder();
            strSql3.Append("delete from " + databaseprefix + "user_attach_log");
            strSql3.Append(" where user_id=@0");
            WriteDataBase.Execute(strSql3.ToString(), id);

            //ɾ������Ϣ
            StringBuilder strSql4 = new StringBuilder();
            strSql4.Append("delete from " + databaseprefix + "user_message ");
            strSql4.Append(" where post_user_name=@0 or accept_user_name=@1");
            WriteDataBase.Execute(strSql4.ToString(), model.user_name, model.user_name);

            //ɾ��������
            StringBuilder strSql5 = new StringBuilder();
            strSql5.Append("delete from " + databaseprefix + "user_code ");
            strSql5.Append(" where user_id=@0");
            WriteDataBase.Execute(strSql5.ToString(), id);

            //ɾ����¼��־
            StringBuilder strSql6 = new StringBuilder();
            strSql6.Append("delete from " + databaseprefix + "user_login_log ");
            strSql6.Append(" where user_id=@0");
            WriteDataBase.Execute(strSql6.ToString(), id);

            //ɾ���û���ַ��
            StringBuilder strSql7 = new StringBuilder();
            strSql7.Append("delete from " + databaseprefix + "user_addr_book ");
            strSql7.Append(" where user_id=@0");
            WriteDataBase.Execute(strSql7.ToString(), id);

            //ɾ��OAuth��Ȩ�û���Ϣ
            StringBuilder strSql8 = new StringBuilder();
            strSql8.Append("delete from " + databaseprefix + "user_oauth ");
            strSql8.Append(" where user_id=@0");
            WriteDataBase.Execute(strSql8.ToString(), id);

            //ɾ���û���ֵ��
            StringBuilder strSql9 = new StringBuilder();
            strSql9.Append("delete from " + databaseprefix + "user_recharge ");
            strSql9.Append(" where user_id=@0");
            WriteDataBase.Execute(strSql9.ToString(), id);

            //ɾ���û�����
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "users ");
            strSql.Append(" where id=@0");
            int rowsAffected = WriteDataBase.Execute(strSql.ToString(), id);
            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// �Ƿ���ڼ�¼
        /// </summary>
        /// <param name="sqlwhere">����</param>
        /// <returns>Users</returns>
        public Model.users GetModel(string sqlwhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 * from " + databaseprefix + "users ");
            if (string.IsNullOrEmpty(sqlwhere))
            {
                strSql.Append(" where 1=1");
            }
            else
            {
                strSql.Append(" where " + sqlwhere + "");
            }
            return ReadDataBase.Query<Model.users>(strSql.ToString()).FirstOrDefault();

        }

        /// <summary>
        /// �����û�������һ��ʵ��
        /// </summary>
        public Model.users GetNmaeModel(string user_name)
        {
            var user = GetModel(" user_name='" + user_name + "' and status<3");
            return user == null ? new Model.users() : user;
        }

        /// <summary>
        /// �����û���ȡ��Salt
        /// </summary>
        public string GetSalt(string user_name)
        {
            //�����û���ȡ��Salt
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 salt from " + databaseprefix + "users");
            strSql.Append(" where (user_name=@0 or mobile=@0)");
            string salt = ReadDataBase.ExecuteScalar<string>(strSql.ToString(), user_name);
            if (!string.IsNullOrEmpty(salt))
            {
                return salt;
            }
            //�������ֻ���ȡ��Salt
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("select top 1 salt from " + databaseprefix + "users");
            strSql1.Append(" where mobile=@0");
            salt = ReadDataBase.ExecuteScalar<string>(strSql1.ToString(), user_name);
            if (!string.IsNullOrEmpty(salt))
            {
                return salt;
            }
            //����������ȡ��Salt
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("select top 1 salt from " + databaseprefix + "users");
            strSql2.Append(" where email=@0");
            salt = ReadDataBase.ExecuteScalar<string>(strSql2.ToString(), user_name);
            if (!string.IsNullOrEmpty(salt))
            {
                return salt;
            }
            return string.Empty;
        }
        #endregion

        #region ��չ����================================
        /// <summary>
        /// ���Email�Ƿ����
        /// </summary>
        public bool ExistsEmail(string email)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "users");
            strSql.Append(" where email=@0 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), email) > 0;
        }

        /// <summary>
        /// ����ֻ������Ƿ����
        /// </summary>
        public bool ExistsMobile(string mobile)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "users");
            strSql.Append(" where mobile=@0 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), mobile) > 0;
        }
        
        /// <summary>
        /// �޸�һ������
        /// </summary>
        public int UpdateField(int id, string strValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + databaseprefix + "users set " + strValue);
            strSql.Append(" where id=" + id);
            return WriteDataBase.Execute(strSql.ToString());
        }
        
        #endregion
    }
}