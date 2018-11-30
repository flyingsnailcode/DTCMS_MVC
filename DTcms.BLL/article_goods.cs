using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
    public partial class article_goods : Services<Model.article_goods>
    {
        private DAL.article_goods dal = new DAL.article_goods(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int channel_id, int article_id, int id)
        {
            return dal.Exists(channel_id, article_id, id);
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 根据规格列表查询商品实体
        /// </summary>
        public Model.article_goods GetModel(int channel_id, int article_id, string spec_ids)
        {
            return dal.GetModel(channel_id, article_id, spec_ids);
        }
        #endregion
    }
}
