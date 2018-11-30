using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;

namespace DTcms.BLL
{
    /// <summary>
    /// ��չ���Ա�
    /// </summary>
    public partial class article_attribute_field : Services<Model.article_attribute_field>
    {
        private DAL.article_attribute_field dal = new DAL.article_attribute_field(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        /// <summary>
        /// ��ѯ�Ƿ������
        /// </summary>
        public bool Exists(string column_name)
        {
            return dal.Exists(column_name);
        }

        /// <summary>
        /// ��������б�
        /// </summary>
        public List<Model.article_attribute_field> GetModelList(int channel_id, string strWhere)
        {
            DataSet ds = dal.GetList(channel_id, strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        

        /// <summary>
        /// ���ǰ��������
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(Top, strWhere, filedOrder);
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public override int Add(Model.article_attribute_field model)
        {
            switch (model.control_type)
            {
                case "single-text": //�����ı�
                    if (model.data_length > 0 && model.data_length <= 4000)
                    {
                        model.data_type = "nvarchar(" + model.data_length + ")";
                    }
                    else if (model.data_length > 4000)
                    {
                        model.data_type = "ntext";
                    }
                    else
                    {
                        model.data_length = 50;
                        model.data_type = "nvarchar(50)";
                    }
                    break;
                case "multi-text": //�����ı�
                    goto case "single-text";
                case "editor": //�༭��
                    model.data_type = "ntext";
                    break;
                case "images": //ͼƬ
                    model.data_type = "nvarchar(255)";
                    break;
                case "video": //��Ƶ
                    model.data_type = "nvarchar(255)";
                    break;
                case "number": //����
                    if (model.data_place > 0)
                    {
                        model.data_type = "decimal(9," + model.data_place + ")";
                    }
                    else
                    {
                        model.data_type = "int";
                    }
                    break;
                case "datetime": //ʱ������
                    model.data_type = "datetime";
                    break;
                case "checkbox": //��ѡ��
                    model.data_type = "tinyint";
                    break;
                case "multi-radio": //���ѡ
                    if (model.data_type == "int")
                    {
                        model.data_length = 4;
                        model.data_type = "int";
                    }
                    else
                    {
                        if (model.data_length > 0 && model.data_length <= 4000)
                        {
                            model.data_type = "nvarchar(" + model.data_length + ")";
                        }
                        else if (model.data_length > 4000)
                        {
                            model.data_type = "ntext";
                        }
                        else
                        {
                            model.data_length = 50;
                            model.data_type = "nvarchar(50)";
                        }
                    }

                    break;
                case "multi-checkbox": //�����ѡ
                    goto case "single-text";
                case "similar": // ͬ���Ƽ�
                    if (model.data_length > 0 && model.data_length <= 4000)
                    {
                        model.data_type = "nvarchar(" + model.data_length + ")";
                    }
                    else
                    {
                        model.data_length = 50;
                        model.data_type = "nvarchar(50)";
                    }
                    break;
            }
            return dal.Add(model);
        }

        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public override bool Update(Model.article_attribute_field model)
        {
            switch (model.control_type)
            {
                case "single-text": //�����ı�
                    if (model.data_length > 0 && model.data_length <= 4000)
                    {
                        model.data_type = "nvarchar(" + model.data_length + ")";
                    }
                    else if (model.data_length > 4000)
                    {
                        model.data_type = "ntext";
                    }
                    else
                    {
                        model.data_length = 50;
                        model.data_type = "nvarchar(50)";
                    }
                    break;
                case "multi-text": //�����ı�
                    goto case "single-text";
                case "editor": //�༭��
                    model.data_type = "ntext";
                    break;
                case "images": //ͼƬ
                    model.data_type = "nvarchar(255)";
                    break;
                case "video": //��Ƶ
                    model.data_type = "nvarchar(255)";
                    break;
                case "number": //����
                    if (model.data_place > 0)
                    {
                        model.data_type = "decimal(9," + model.data_place + ")";
                    }
                    else
                    {
                        model.data_type = "int";
                    }
                    break;
                case "datetime": //ʱ������
                    model.data_type = "datetime";
                    break;
                case "checkbox": //��ѡ��
                    model.data_type = "tinyint";
                    break;
                case "multi-radio": //���ѡ
                    if (model.data_type == "int")
                    {
                        model.data_length = 4;
                        model.data_type = "int";
                    }
                    else
                    {
                        if (model.data_length > 0 && model.data_length <= 4000)
                        {
                            model.data_type = "nvarchar(" + model.data_length + ")";
                        }
                        else if (model.data_length > 4000)
                        {
                            model.data_type = "ntext";
                        }
                        else
                        {
                            model.data_length = 50;
                            model.data_type = "nvarchar(50)";
                        }
                    }

                    break;
                case "multi-checkbox": //�����ѡ
                    goto case "single-text";
                case "similar": // ͬ���Ƽ�
                    model.data_type = "nvarchar(" + model.data_length + ")";
                    break;
            }
            return dal.Update(model);
        }
        #endregion

        #region ˽�з���===============================
        /// <summary>
        /// ��ȡ��չ�ֶζԳ�ֵ
        /// </summary>
        public Dictionary<string, string> GetFields(int channel_id, int article_id, string strWhere)
        {
            return dal.GetFields(channel_id, article_id, strWhere);
        }

        /// <summary>
        /// ��������б�
        /// </summary>
        private List<Model.article_attribute_field> DataTableToList(DataTable dt)
        {
            List<Model.article_attribute_field> modelList = new List<Model.article_attribute_field>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                for (int n = 0; n < rowsCount; n++)
                {
                    modelList.Add(dal.DataRowToModel(dt.Rows[n]));
                }
            }
            return modelList;
        }
        #endregion
    }
}