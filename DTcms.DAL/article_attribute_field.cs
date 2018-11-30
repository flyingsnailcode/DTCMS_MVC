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
    /// ��չ�������ݷ�����:article_attribute_field
    /// </summary>
    public partial class article_attribute_field : DapperRepository<Model.article_attribute_field>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public article_attribute_field(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region ��չ����================================
        /// <summary>
        /// ��ѯ�Ƿ������
        /// </summary>
        public bool Exists(string column_name)
        {
            //����Ƿ��������ֶ���ͬ
            Model.article artModel = new Model.article();
            //���÷��������Ե����й�������
            Type modelType = artModel.GetType();
            PropertyInfo[] proInfo = modelType.GetProperties();
            foreach (PropertyInfo pi in proInfo)
            {
                if (pi.Name.ToLower() == column_name.ToLower())
                {
                    return true;
                }
            }
            //����Ƿ�����չ�ֶα�����ͬ
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  " + databaseprefix + "article_attribute_field");
            strSql.Append(" where name=@0 ");
            return ReadDataBase.ExecuteScalar<int>(strSql.ToString(), column_name) > 0;
        }

        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(int id)
        {
            Model.article_attribute_field model = GetModel(id);//ȡ����չ�ֶ�ʵ��
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //ɾ����������Ƶ�����ݱ������
                        DataTable dt = new DAL.site_channel(databaseprefix).GetFieldList(conn, trans, id).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                //������޸�Ƶ�����ݱ����
                                int rowsCount = ReadDataBase.ExecuteScalar<int>(conn, trans, "select count(1) from syscolumns where id=object_id('" + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dr["name"].ToString() + "') and name='" + model.name + "'");
                                if (rowsCount > 0)
                                {
                                    //ɾ��Ƶ�����ݱ�һ��
                                    WriteDataBase.Execute(conn, trans, "alter table " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dr["name"].ToString() + " drop column " + model.name);
                                }
                            }
                        }

                        //ɾ��Ƶ�������ֶα�
                        StringBuilder strSql1 = new StringBuilder();
                        strSql1.Append("delete from " + databaseprefix + "site_channel_field");
                        strSql1.Append(" where field_id=@0");
                        WriteDataBase.Execute(conn, trans, strSql1.ToString(), id);

                        //ɾ����չ�ֶ�����
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("delete from " + databaseprefix + "article_attribute_field");
                        strSql.Append(" where id=@0");
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), id);

                        trans.Commit();//�ύ����
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();//�ع�����
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// ���ǰ��������
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM  " + databaseprefix + "article_attribute_field");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// ���Ƶ����Ӧ������
        /// </summary>
        public DataSet GetList(int channel_id, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select A.* ");
            strSql.Append(" FROM " + databaseprefix + "article_attribute_field as A INNER JOIN " + databaseprefix + "site_channel_field as S ON A.id=S.field_id");
            strSql.Append(" where S.channel_id=" + channel_id);
            if (strWhere.Trim() != "")
            {
                strSql.Append(" and " + strWhere);
            }
            strSql.Append(" order by A.is_sys desc,A.sort_id asc,A.id desc");
            return ReadDataBase.QueryFillDataSet(strSql.ToString());
        }

        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public override Model.article_attribute_field GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.article_attribute_field model = new Model.article_attribute_field();
            //���÷��������Ե����й�������
            Type modelType = model.GetType();
            PropertyInfo[] pros = modelType.GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//ƴ���ֶ�
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "article_attribute_field");
            strSql.Append(" where id="+ id);
            DataTable dt = WriteDataBase.QueryFillDataSet(strSql.ToString()).Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows[0].Table.Columns.Count; i++)
                {
                    //����ʵ���Ƿ�����б���ͬ�Ĺ�������
                    PropertyInfo proInfo = modelType.GetProperty(dt.Rows[0].Table.Columns[i].ColumnName);
                    if (proInfo != null && dt.Rows[0][i] != DBNull.Value)
                    {
                        proInfo.SetValue(model, dt.Rows[0][i], null);//������ֵ��������ֵ
                    }
                }
                return model;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public bool Update(Model.article_attribute_field model)
        {
            int i = 0;
            Model.article_attribute_field oldModel = GetModel(model.id);//ȡ���ɵ�����
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //�޸�������Ϣ
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();
                        //���÷��������Ե����й�������
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("update  " + databaseprefix + "article_attribute_field set ");
                        foreach (PropertyInfo pi in pros)
                        {
                            //�������������׷��sql�ַ���
                            if (!pi.Name.Equals("id"))
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
                        strSql.Append(" where id=@" + i + " ");
                        paras.Add(model.id);
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());

                        //����ֶ������������ޱ仯
                        if (oldModel.name.ToLower() != model.name.ToLower() || oldModel.data_type.ToLower() != model.data_type.ToLower())
                        {
                            DataTable dt = new DAL.site_channel(databaseprefix).GetFieldList(conn, trans, model.id).Tables[0];
                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    //������޸�Ƶ�����ݱ����
                                    int rowsCount = ReadDataBase.ExecuteScalar<int>(conn, trans, "select count(1) from syscolumns where id=object_id('" + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dr["name"].ToString() + "') and name='" + oldModel.name + "'");
                                    if (rowsCount > 0)
                                    {
                                        //�޸�����������
                                        if (oldModel.data_type.ToLower() != model.data_type.ToLower())
                                        {
                                            WriteDataBase.Execute(conn, trans, "alter table " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dr["name"].ToString() + " alter column " + oldModel.name + " " + model.data_type);
                                        }
                                        //�޸�����
                                        if (oldModel.name.ToLower() != model.name.ToLower())
                                        {
                                            WriteDataBase.Execute(conn, trans, "exec sp_rename '" + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + dr["name"].ToString() + "." + oldModel.name + "','" + model.name + "','column'");
                                        }
                                    }
                                }
                            }
                        }

                        trans.Commit();//�ύ����
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();//�ع�����
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region ��չ����========================
        /// <summary>
        /// ��ȡ��չ�ֶζԳ�ֵ
        /// </summary>
        public Dictionary<string, string> GetFields(int channel_id, int article_id, string strWhere)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            DataTable dt = GetList(channel_id, strWhere).Tables[0];
            if (dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append(dr["name"].ToString() + ",");
                }
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select top 1 " + Utils.DelLastComma(sb.ToString()) + " from " + databaseprefix + "article_attribute_value ");
                strSql.Append(" where article_id=" + article_id);
                DataSet ds = ReadDataBase.QueryFillDataSet(strSql.ToString());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (ds.Tables[0].Rows[0][dr["name"].ToString()] != null)
                        {
                            dic.Add(dr["name"].ToString(), ds.Tables[0].Rows[0][dr["name"].ToString()].ToString());
                        }
                        else
                        {
                            dic.Add(dr["name"].ToString(), "");
                        }
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// ��ȡƵ������չ�ֶ�
        /// </summary>
        public Dictionary<string, string> GetFields(int channel_id)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            DataTable dt = GetList(channel_id, string.Empty).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dic.Add(dr["name"].ToString(), string.Empty);
                }
            }
            return dic;
        }
        #endregion

        #region ˽�з���===============================
        /// <summary>
        /// ������ת��Ϊʵ��
        /// </summary>
        public Model.article_attribute_field DataRowToModel(DataRow row)
        {
            Model.article_attribute_field model = new Model.article_attribute_field();
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
            }
            return model;
        }

        #endregion
    }
}