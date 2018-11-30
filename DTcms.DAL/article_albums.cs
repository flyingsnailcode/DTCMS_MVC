using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Reflection;
using Dapper;
using System.Linq;

namespace DTcms.DAL
{
	/// <summary>
	/// ͼƬ���
	/// </summary>
	public partial class article_albums : DapperRepository<Model.article_albums>
    {
        private string databaseprefix; //���ݿ����ǰ׺
        public article_albums(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region ��չ����================================
        /// <summary>
        /// ��������б�
        /// </summary>
        public DataTable GetImagesList(int article_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,article_id,thumb_path,original_path,sort_id,remark,add_time,channel_id ");
            strSql.Append(" FROM " + databaseprefix + "article_albums ");
            strSql.Append(" where article_id=" + article_id);
            strSql.Append(" order by sort_id asc, id asc");
            DataSet ds = WriteDataBase.QueryFillDataSet(strSql.ToString());
            return ds.Tables[0];
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public void Add(IDbConnection conn, IDbTransaction trans, List<Model.article_albums> models, int channel_id, int article_id)
        {
            int i = 0;
            if (models != null)
            {
                StringBuilder strSql;
                StringBuilder str1; ;//�����ֶ�
                StringBuilder str2;//���ݲ���
                foreach (Model.article_albums modelt in models)
                {
                    i = 0;
                    strSql = new StringBuilder();
                    str1 = new StringBuilder();
                    str2 = new StringBuilder();
                    //���÷��������Ե����й�������
                    PropertyInfo[] pros = modelt.GetType().GetProperties();
                    List<object> paras = new List<object>();
                    strSql.Append("insert into " + databaseprefix + "article_albums(");
                    foreach (PropertyInfo pi in pros)
                    {
                        //�������������׷��sql�ַ���
                        if (!pi.Name.Equals("id"))
                        {
                            //�ж�����ֵ�Ƿ�Ϊ��
                            if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                            {
                                str1.Append(pi.Name + ",");//ƴ���ֶ�
                                str2.Append("@" + i + ",");//��������
                                i++;
                                switch (pi.Name)
                                {
                                    case "channel_id":
                                        paras.Add(channel_id);
                                        break;
                                    case "article_id":
                                        paras.Add(article_id);//�ղ��������ID
                                        break;
                                    default:
                                        paras.Add(pi.GetValue(modelt, null));//�Բ�����ֵ
                                        break;
                                }
                            }
                        }
                    }
                    strSql.Append(str1.ToString().Trim(','));
                    strSql.Append(") values (");
                    strSql.Append(str2.ToString().Trim(','));
                    strSql.Append(") ");
                    WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());//������
                }
            }
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public void Update(IDbConnection conn, IDbTransaction trans, List<Model.article_albums> models, int channel_id, int article_id)
        {
            int i = 0;
            //ɾ�����Ƴ���ͼƬ
            DeleteList(conn, trans, models, channel_id, article_id);
            //���/�޸����
            if (models != null)
            {
                StringBuilder strSql;
                StringBuilder str1;//�����ֶ�
                StringBuilder str2;//���ݲ���
                foreach (Model.article_albums modelt in models)
                {
                    i = 0;
                    strSql = new StringBuilder();
                    str1 = new StringBuilder();
                    str2 = new StringBuilder();
                    //���÷��������Ե����й�������
                    PropertyInfo[] pros = modelt.GetType().GetProperties();
                    List<object> paras = new List<object>();
                    if (modelt.id > 0)
                    {
                        strSql.Append("update " + databaseprefix + "article_albums set ");
                        foreach (PropertyInfo pi in pros)
                        {
                            //�������������׷��sql�ַ���
                            if (!pi.Name.Equals("id"))
                            {
                                //�ж�����ֵ�Ƿ�Ϊ��
                                if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                {
                                    str1.Append(pi.Name + "=@" + i + ",");//��������
                                    i++;
                                    paras.Add(pi.GetValue(modelt, null));//�Բ�����ֵ
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(" where id=@"+i+"");
                        paras.Add(modelt.id);
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());//������
                    }
                    else
                    {
                        i = 0;
                        strSql.Append("insert into " + databaseprefix + "article_albums(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //�������������׷��sql�ַ���
                            if (!pi.Name.Equals("id"))
                            {
                                //�ж�����ֵ�Ƿ�Ϊ��
                                if (pi.GetValue(modelt, null) != null && !pi.GetValue(modelt, null).ToString().Equals(""))
                                {
                                    str1.Append(pi.Name + ",");//ƴ���ֶ�
                                    str2.Append("@" + i + ",");//��������
                                    i++;
                                    paras.Add(pi.GetValue(modelt, null));//�Բ�����ֵ
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(") values (");
                        strSql.Append(str2.ToString().Trim(','));
                        strSql.Append(") ");
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), paras.ToArray());//������
                    }
                }
            }
        }

        /// <summary>
        /// ���Ҳ����ڵ�ͼƬ��ɾ�����Ƴ���ͼƬ������
        /// </summary>
        public void DeleteList(IDbConnection conn, IDbTransaction trans, List<Model.article_albums> models, int channel_id, int article_id)
        {
            StringBuilder idList = new StringBuilder();
            if (models != null)
            {
                foreach (Model.article_albums modelt in models)
                {
                    if (modelt.id > 0)
                    {
                        idList.Append(modelt.id + ",");
                    }
                }
            }
            string delIds = idList.ToString().TrimEnd(',');
            string strwhere = "channel_id=" + channel_id + " and article_id=" + article_id;
            if (!string.IsNullOrEmpty(delIds))
            {
                strwhere += " and id not in(" + delIds + ")";
            }
            List<Model.article_albums> albums = new List<Model.article_albums>();
            albums = WriteDataBase.Query<Model.article_albums>(conn, trans, Sql.Builder.Select("channel_id,id,thumb_path,original_path").From(TableName).Where(strwhere)).ToList();
            foreach (var dr in albums)
            {
                int rows = WriteDataBase.Execute(conn, trans, "delete from " + databaseprefix + "article_albums where id=" + dr.id); //ɾ�����ݿ�
                if (rows > 0)
                {
                    FileHelper.DeleteFile(dr.thumb_path); //ɾ������ͼ
                    FileHelper.DeleteFile(dr.original_path); //ɾ��ԭͼ
                }
            }
        }

        /// <summary>
        /// ɾ�����ͼƬ
        /// </summary>
        public void DeleteFile(List<Model.article_albums> models)
        {
            if (models != null)
            {
                foreach (Model.article_albums modelt in models)
                {
                    FileHelper.DeleteFile(modelt.thumb_path);
                    FileHelper.DeleteFile(modelt.original_path);
                }
            }
        }

        /// <summary>
        /// ��������б�
        /// </summary>
        public List<Model.article_albums> GetList(int channel_id, int article_id)
        {
            List<Model.article_albums> modelList = new List<Model.article_albums>();
            modelList = GetModelList(0, "channel_id=" + channel_id + " and article_id=" + article_id, "", "");
            return modelList;
        }
        #endregion

    }
}

