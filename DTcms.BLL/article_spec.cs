using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 商品规格
    /// </summary>
    public partial class article_spec : Services<Model.article_spec>
    {
        private DAL.article_spec dal = new DAL.article_spec(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public override int Add(Model.article_spec model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public override bool Update(Model.article_spec model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public override Model.article_spec GetModel(int id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// 根据频道和分类ID筛选出规格(慎用)
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="category_id">类别ID</param>
        /// <returns>DataSet</returns>
        public DataSet GetParentList(string channel_name, int category_id)
        {
            return dal.GetParentList(channel_name, category_id);
        }

        /// <summary>
        /// 返回栏目规格列表
        /// </summary>
        public DataSet GetCategorySpecList(int category_id)
        {
            return dal.GetCategorySpecList(category_id);
        }
        #endregion
    }
}