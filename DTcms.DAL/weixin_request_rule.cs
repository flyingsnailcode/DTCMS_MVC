using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Reflection;

namespace DTcms.DAL
{
	/// <summary>
    /// ���ݷ�����:����ظ����
	/// </summary>
	public class weixin_request_rule : DapperRepository<Model.weixin_request_rule>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public weixin_request_rule(string _databaseprefix)
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
            //ɾ���������ݱ�
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("delete from " + databaseprefix + "weixin_request_content");
            strSql2.Append(" where rule_id=@0 ");
            sqllist.Add(strSql2.ToString());

            //ɾ����������
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "weixin_request_rule");
            strSql.Append(" where id=@0");
            sqllist.Add(strSql.ToString());

            return WriteDataBase.ExecuteSqlTran(sqllist, id) >0;
        }

        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public Model.weixin_request_rule GetModel(int account_id, int request_type)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.weixin_request_rule model = new Model.weixin_request_rule();
            //���÷��������Ե����й�������
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                //ƴ���ֶΣ�����List<T>
                if (!p.Name.Equals("values") && !p.Name.Equals("contents"))
                {
                    str1.Append(p.Name + ",");
                }
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "weixin_request_rule");
            strSql.AppendFormat(" where account_id={0} and request_type='{1}'", account_id, request_type);

            DataSet ds = WriteDataBase.QueryFillDataSet(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
		/// ����һ������
		/// </summary>
		public int Add(Model.weixin_request_rule model)
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
                        strSql.Append("insert into " + databaseprefix + "weixin_request_rule(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //�������������List<T>��׷��sql�ַ���
                            if (!pi.Name.Equals("id") && !pi.Name.Equals("contents"))
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

                        #region �������ݱ���Ϣ====================
                        if (model.contents != null)
                        {
                            i = 0;
                            StringBuilder strSql2; //SQL�ַ���
                            StringBuilder str21; //���ݿ��ֶ�
                            StringBuilder str22; //��������
                            foreach (Model.weixin_request_content modelt in model.contents)
                            {
                                strSql2 = new StringBuilder();
                                str21 = new StringBuilder();
                                str22 = new StringBuilder();
                                PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                List<object> paras2 = new List<object>();
                                strSql2.Append("insert into " + databaseprefix + "weixin_request_content(");
                                foreach (PropertyInfo pi in pros2)
                                {
                                    if (!pi.Name.Equals("id"))
                                    {
                                        if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                        {
                                            str21.Append(pi.Name + ",");
                                            str22.Append("@" + i + ",");
                                            i++;
                                            if (pi.Name.Equals("rule_id"))
                                            {
                                                paras2.Add(model.id); //���ղ���ĸ�ID��ֵ
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

                        trans.Commit();//�ύ����
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();//�ع�����
                        return 0;
                    }
                }
            }
            return model.id;
        }

        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public override Model.weixin_request_rule GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.weixin_request_rule model = new Model.weixin_request_rule();
            //���÷��������Ե����й�������
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                //ƴ���ֶΣ�����List<T>
                if (!p.Name.Equals("values") && !p.Name.Equals("contents"))
                {
                    str1.Append(p.Name + ",");
                }
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "weixin_request_rule");
            strSql.Append(" where id="+ id);
            DataTable dt = WriteDataBase.QueryFillDataSet(strSql.ToString()).Tables[0];

            if (dt.Rows.Count > 0)
            {
                return DataRowToModel(dt.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
		/// ����һ������
		/// </summary>
        public bool Update(Model.weixin_request_rule model)
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
                        strSql.Append("update  " + databaseprefix + "weixin_request_rule set ");
                        foreach (PropertyInfo pi in pros)
                        {
                            //�������������List<T>��׷��sql�ַ���
                            if (!pi.Name.Equals("id") && !typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
                            {
                                //�ж�����ֵ�Ƿ�Ϊ��
                                if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                                {
                                    //��������
                                    str1.Append(pi.Name + "=@" + i + ",");
                                    i++;
                                    //�Բ�����ֵ
                                    paras.Add(pi.GetValue(model, null));
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(" where id=@" + i + " ");
                        paras.Add(model.id);
                        WriteDataBase.Execute(conn, trans,strSql.ToString(), paras.ToArray());
                        #endregion

                        #region �������ݱ���Ϣ====================
                        //��ɾ���Ĺ�������
                        WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "weixin_request_content where rule_id=" + model.id);
                        //������ӹ�������
                        if (model.contents != null)
                        {
                            i = 0;
                            StringBuilder strSql2; //SQL�ַ���
                            StringBuilder str21; //���ݿ��ֶ�
                            StringBuilder str22; //��������
                            foreach (Model.weixin_request_content modelt in model.contents)
                            {
                                strSql2 = new StringBuilder();
                                str21 = new StringBuilder();
                                str22 = new StringBuilder();
                                PropertyInfo[] pros2 = modelt.GetType().GetProperties();
                                List<object> paras2 = new List<object>();
                                strSql2.Append("insert into " + databaseprefix + "weixin_request_content(");
                                foreach (PropertyInfo pi in pros2)
                                {
                                    if (!pi.Name.Equals("id"))
                                    {
                                        if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                        {
                                            str21.Append(pi.Name + ",");
                                            str22.Append("@" + i + ",");
                                            i++;
                                            if (pi.Name.Equals("rule_id"))
                                            {
                                                paras2.Add(model.id); //������ID��ֵ
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

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region ΢��ͨѶ����============================
        /// <summary>
        /// �õ�����ID�Լ��ظ�����
        /// </summary>
        public int GetRuleIdAndResponseType(int account_id, string strWhere, out int response_type)
        {
            int rule_id = 0;
            response_type = 0;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 id,response_type from " + databaseprefix + "weixin_request_rule");
            strSql.Append(" where account_id=" + account_id);
            if (strWhere != null && strWhere.Length > 0)
            {
                strSql.Append(" and " + strWhere);
            }
            SqlDataReader sr = ReadDataBase.ExecuteScalar<SqlDataReader>(strSql.ToString());

            while (sr.Read())
            {
                rule_id = int.Parse(sr["id"].ToString());
                response_type = int.Parse(sr["response_type"].ToString());
            }
            sr.Close();

            return rule_id;
        }

        /// <summary>
        /// �õ��ؽ��ֲ�ѯ�Ĺ���ID���ظ�����(�������Ч�ʿ�ʹ�ô洢����)
        /// </summary>
        public int GetKeywordsRuleId(int account_id, string keywords, out int response_type)
        {
            int rule_id = 0;
            //��ȷƥ��
            StringBuilder strSql3 = new StringBuilder();
            strSql3.Append("select top 1 id,response_type from " + databaseprefix + "weixin_request_rule");
            strSql3.Append(" where account_id=" + account_id + " and request_type=1");
            strSql3.Append(" and(keywords like '" + keywords + "|%' or keywords='%|" + keywords + "' or keywords like '%|" + keywords + "|%' or keywords='" + keywords + "')");
            strSql3.Append(" order by sort_id asc,add_time desc");
            DataSet ds3 = WriteDataBase.QueryFillDataSet(strSql3.ToString());
            if (ds3.Tables[0].Rows.Count > 0)
            {
                rule_id = int.Parse(ds3.Tables[0].Rows[0][0].ToString());
                response_type = int.Parse(ds3.Tables[0].Rows[0][1].ToString());
                return rule_id;
            }
            //ģ��ƥ��
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("select top 1 id,response_type from " + databaseprefix + "weixin_request_rule");
            strSql2.Append(" where account_id=" + account_id + " and request_type=1 and keywords like '%" + keywords + "%'");
            strSql2.Append(" order by sort_id asc,add_time desc");
            DataSet ds2 = WriteDataBase.QueryFillDataSet(strSql2.ToString());
            if (ds2.Tables[0].Rows.Count > 0)
            {
                rule_id = int.Parse(ds2.Tables[0].Rows[0][0].ToString());
                response_type = int.Parse(ds2.Tables[0].Rows[0][1].ToString());
                return rule_id;
            }
            //Ĭ�ϻظ�
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("select top 1 id,response_type from " + databaseprefix + "weixin_request_rule");
            strSql1.Append(" where account_id=" + account_id + " and request_type=0");
            strSql1.Append(" order by sort_id asc,add_time desc");
            DataSet ds1 = WriteDataBase.QueryFillDataSet(strSql1.ToString());
            if (ds1.Tables[0].Rows.Count > 0)
            {
                rule_id = int.Parse(ds1.Tables[0].Rows[0][0].ToString());
                response_type = int.Parse(ds1.Tables[0].Rows[0][1].ToString());
                return rule_id;
            }
            response_type = 0;
            return rule_id;
        }
        #endregion

        /// <summary>
        /// ������ת��ʵ��
        /// </summary>
        public Model.weixin_request_rule DataRowToModel(DataRow row)
        {
            Model.weixin_request_rule model = new Model.weixin_request_rule();
            if (row != null)
            {
                #region ������Ϣ======================
                //���÷��������Ե����й�������
                Type modelType = model.GetType();
                for (int i = 0; i < row.Table.Columns.Count; i++)
                {
                    //����ʵ���Ƿ�����б���ͬ�Ĺ�������
                    PropertyInfo proInfo = modelType.GetProperty(row.Table.Columns[i].ColumnName);
                    if (proInfo != null && row[i] != DBNull.Value)
                    {
                        proInfo.SetValue(model, row[i], null);//������ֵ��������ֵ
                    }
                }
                #endregion

                #region �ӱ���Ϣ======================
                StringBuilder strSql1 = new StringBuilder();
                strSql1.Append("select * from " + databaseprefix + "weixin_request_content");
                strSql1.Append(" where rule_id="+ model.id);
                DataTable dt1 = WriteDataBase.QueryFillDataSet(strSql1.ToString()).Tables[0];

                if (dt1.Rows.Count > 0)
                {
                    int rowsCount = dt1.Rows.Count;
                    List<Model.weixin_request_content> models = new List<Model.weixin_request_content>();
                    Model.weixin_request_content modelt;
                    for (int n = 0; n < rowsCount; n++)
                    {
                        modelt = new Model.weixin_request_content();
                        Type modeltType = modelt.GetType();
                        for (int i = 0; i < dt1.Rows[n].Table.Columns.Count; i++)
                        {
                            PropertyInfo proInfo = modeltType.GetProperty(dt1.Rows[n].Table.Columns[i].ColumnName);
                            if (proInfo != null && dt1.Rows[n][i] != DBNull.Value)
                            {
                                proInfo.SetValue(modelt, dt1.Rows[n][i], null);
                            }
                        }
                        models.Add(modelt);
                    }
                    model.contents = models;
                }
                #endregion
            }
            return model;
        }
    }
}

