using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;

namespace DTcms.BLL
{
    /// <summary>
    /// 扩展属性表
    /// </summary>
    public partial class article_attribute_field : Services<Model.article_attribute_field>
    {
        private DAL.article_attribute_field dal = new DAL.article_attribute_field(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 查询是否存在列
        /// </summary>
        public bool Exists(string column_name)
        {
            return dal.Exists(column_name);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Model.article_attribute_field> GetModelList(int channel_id, string strWhere)
        {
            DataSet ds = dal.GetList(channel_id, strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(Top, strWhere, filedOrder);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public override int Add(Model.article_attribute_field model)
        {
            switch (model.control_type)
            {
                case "single-text": //单行文本
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
                case "multi-text": //多行文本
                    goto case "single-text";
                case "editor": //编辑器
                    model.data_type = "ntext";
                    break;
                case "images": //图片
                    model.data_type = "nvarchar(255)";
                    break;
                case "video": //视频
                    model.data_type = "nvarchar(255)";
                    break;
                case "number": //数字
                    if (model.data_place > 0)
                    {
                        model.data_type = "decimal(9," + model.data_place + ")";
                    }
                    else
                    {
                        model.data_type = "int";
                    }
                    break;
                case "datetime": //时间日期
                    model.data_type = "datetime";
                    break;
                case "checkbox": //复选框
                    model.data_type = "tinyint";
                    break;
                case "multi-radio": //多项单选
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
                case "multi-checkbox": //多项多选
                    goto case "single-text";
                case "similar": // 同类推荐
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
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public override bool Update(Model.article_attribute_field model)
        {
            switch (model.control_type)
            {
                case "single-text": //单行文本
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
                case "multi-text": //多行文本
                    goto case "single-text";
                case "editor": //编辑器
                    model.data_type = "ntext";
                    break;
                case "images": //图片
                    model.data_type = "nvarchar(255)";
                    break;
                case "video": //视频
                    model.data_type = "nvarchar(255)";
                    break;
                case "number": //数字
                    if (model.data_place > 0)
                    {
                        model.data_type = "decimal(9," + model.data_place + ")";
                    }
                    else
                    {
                        model.data_type = "int";
                    }
                    break;
                case "datetime": //时间日期
                    model.data_type = "datetime";
                    break;
                case "checkbox": //复选框
                    model.data_type = "tinyint";
                    break;
                case "multi-radio": //多项单选
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
                case "multi-checkbox": //多项多选
                    goto case "single-text";
                case "similar": // 同类推荐
                    model.data_type = "nvarchar(" + model.data_length + ")";
                    break;
            }
            return dal.Update(model);
        }
        #endregion

        #region 私有方法===============================
        /// <summary>
        /// 获取扩展字段对称值
        /// </summary>
        public Dictionary<string, string> GetFields(int channel_id, int article_id, string strWhere)
        {
            return dal.GetFields(channel_id, article_id, strWhere);
        }

        /// <summary>
        /// 获得数据列表
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