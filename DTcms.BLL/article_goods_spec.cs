using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
    /// <summary>
    /// 商品对应规格表
    /// </summary>
    public partial class article_goods_spec : Services<Model.article_goods_spec>
    {
        private DAL.article_goods_spec dal = new DAL.article_goods_spec(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Model.article_goods_spec> GetList(int channel_id, int article_id, string strWhere)
        {
            return dal.GetList(channel_id, article_id, strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Model.article_goods_spec> GetList(string channelName, int article_id, string strWhere)
        {
            return dal.GetList(new BLL.site_channel().GetChannelId(channelName), article_id, strWhere);
        }
        #endregion

    }
}
