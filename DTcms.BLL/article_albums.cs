using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 图片列表
    /// </summary>
    public partial class article_albums : Services<Model.article_albums>
    {
        private DAL.article_albums dal = new DAL.article_albums(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        public List<Model.article_albums> GetList(int channel_id, int id)
        {
            return dal.GetList(channel_id, id);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataTable GetImagesList(int article_id)
        {
            return dal.GetImagesList(article_id);
        }
        #endregion
    }
}
