using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace DTcms.DAL
{
    /// <summary>
    /// ���ݷ�����:���ݵ���
    /// </summary>
    public partial class article_zan : DapperRepository<Model.article_zan>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public article_zan(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region ��չ����================================
        /// <summary>
        /// ��ò�ѯ��ҳ����
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + "article_zan");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return ReadDataBase.QueryFillDataSet(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public int Add(Model.article_zan model)
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
                        strSql.Append("insert into  " + databaseprefix + "article_zan(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //�������������׷��sql�ַ���
                            if (!pi.Name.Equals("id"))
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
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return 0;
                    }
                }
            }
            return model.id;
        }
        
        #endregion
    }
}