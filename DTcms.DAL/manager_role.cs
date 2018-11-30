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
    /// ���ݷ�����:�����ɫ
    /// </summary>
    public partial class manager_role : DapperRepository<Model.manager_role>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public manager_role(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region ��չ����================================
        /// <summary>
        /// ɾ��һ�����ݣ����ӱ������������
        /// </summary>
        public bool Delete(int id)
        {
            List<string> sqllist = new List<string>();
            //ɾ�������ɫȨ��
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "manager_role_value ");
            strSql.Append(" where role_id=@0");
            sqllist.Add(strSql.ToString());
            //ɾ�������ɫ
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("delete from " + databaseprefix + "manager_role ");
            strSql2.Append(" where id=@0");
            sqllist.Add(strSql2.ToString());

            int rowsAffected = WriteDataBase.ExecuteSqlTran(sqllist, id);
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
        /// ���ؽ�ɫ����
        /// </summary>
        public string GetTitle(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 role_name from " + databaseprefix + "manager_role");
            strSql.Append(" where id=" + id);
            string title = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
            if (string.IsNullOrEmpty(title))
            {
                return string.Empty;
            }
            return title;
        }
        /// <summary>
        /// ����һ������
        /// </summary>
        public bool Update(Model.manager_role model)
        {
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region ������Ϣ==========================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();
                        //���÷��������Ե����й�������
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("update " + databaseprefix + "manager_role set ");
                        foreach (PropertyInfo pi in pros)
                        {
                            //�������������LIST<T>��׷��sql�ַ���
                            if (!pi.Name.Equals("id") && !typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
                            {
                                //�ж�����ֵ�Ƿ�Ϊ��
                                if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                                {
                                    str1.Append(pi.Name + "=@" + i + ",");//��������
                                    i++;
                                    paras.Add(pi.GetValue(model, null));//�Բ�����ֵ
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(" where id=@"+i+" ");
                        paras.Add(model.id);
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());
                        #endregion

                        #region ��ɫȨ�ޱ���Ϣ====================
                        //ɾ����ɫ���е�Ȩ��
                        StringBuilder strSql1 = new StringBuilder();
                        strSql1.Append("delete from " + databaseprefix + "manager_role_value where role_id=@0");
                        WriteDataBase.Execute(strSql1.ToString(), model.id);

                        //������ӽ�ɫȨ��
                        if (model.manager_role_values != null)
                        {
                            
                            StringBuilder strSql2; //SQL�ַ���
                            StringBuilder str21; //���ݿ��ֶ�
                            StringBuilder str22; //��������
                            foreach (Model.manager_role_value modelt in model.manager_role_values)
                            {
                                i = 0;
                                strSql2 = new StringBuilder();
                                str21 = new StringBuilder();
                                str22 = new StringBuilder();
                                PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                List<object> paras2 = new List<object>();
                                strSql2.Append("insert into " + databaseprefix + "manager_role_value(");
                                foreach (PropertyInfo pi in pros2)
                                {
                                    if (!pi.Name.Equals("id"))
                                    {
                                        if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                        {
                                            str21.Append(pi.Name + ",");
                                            str22.Append("@" + i + ",");
                                            i++;
                                            if (pi.Name.Equals("role_id"))
                                            {
                                                paras2.Add(model.id);//���ս�ɫ��ID��ֵ
                                            }
                                            else
                                            {
                                                paras2.Add(pi.GetValue(modelt, null));
                                            }
                                        }
                                    }
                                }
                                strSql2.Append(str21.ToString().Trim(','));
                                strSql2.Append(") values (");
                                strSql2.Append(str22.ToString().Trim(','));
                                strSql2.Append(") ");
                                WriteDataBase.Execute(conn, trans, strSql2.ToString(), paras2.ToArray());
                            }
                        }
                        #endregion

                        trans.Commit(); //�ύ����
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback(); //�ع�����
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public int Add(Model.manager_role model)
        {
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region ������Ϣ==========================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();//�����ֶ�
                        StringBuilder str2 = new StringBuilder();//���ݲ���
                        //���÷��������Ե����й�������
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("insert into " + databaseprefix + "manager_role(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //�������������LIST<T>��׷��sql�ַ���
                            if (!pi.Name.Equals("id") && !typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
                            {
                                //�ж�����ֵ�Ƿ�Ϊ��
                                if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                                {
                                    str1.Append(pi.Name + ",");//ƴ���ֶ�
                                    str2.Append("@" + i + ",");//��������
                                    i++;
                                    paras.Add(pi.GetValue(model, null));//�Բ�����ֵ
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(") values (");
                        strSql.Append(str2.ToString().Trim(','));
                        strSql.Append(") ");
                        strSql.Append(";SELECT @@@IDENTITY;");
                        object obj = WriteDataBase.ExecuteScalar<object>(conn, trans, strSql.ToString(), paras.ToArray());
                        model.id = Convert.ToInt32(obj);
                        #endregion

                        #region ��ɫȨ�ޱ���Ϣ====================
                        if (model.manager_role_values != null)
                        {
                            
                            StringBuilder strSql2; //SQL�ַ���
                            StringBuilder str21; //���ݿ��ֶ�
                            StringBuilder str22; //��������
                            foreach (Model.manager_role_value modelt in model.manager_role_values)
                            {
                                i = 0;
                                strSql2 = new StringBuilder();
                                str21 = new StringBuilder();
                                str22 = new StringBuilder();
                                PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                List<object> paras2 = new List<object>();
                                strSql2.Append("insert into " + databaseprefix + "manager_role_value(");
                                foreach (PropertyInfo pi in pros2)
                                {
                                    if (!pi.Name.Equals("id"))
                                    {
                                        if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                        {
                                            str21.Append(pi.Name + ",");
                                            str22.Append("@" + i + ",");
                                            i++;
                                            if (pi.Name.Equals("role_id"))
                                            {
                                                paras2.Add(model.id);//���ղ���ĸ�ID��ֵ
                                            }
                                            else
                                            {
                                                paras2.Add(pi.GetValue(modelt, null));
                                            }
                                        }
                                    }
                                }
                                strSql2.Append(str21.ToString().Trim(','));
                                strSql2.Append(") values (");
                                strSql2.Append(str22.ToString().Trim(','));
                                strSql2.Append(") ");
                                WriteDataBase.Execute(conn, trans, strSql2.ToString(), paras2.ToArray());
                            }
                        }
                        #endregion

                        trans.Commit(); //�ύ����
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback(); //�ع�����
                        return 0;
                    }
                }
            }
            return model.id;
        }
        #endregion
    }
}