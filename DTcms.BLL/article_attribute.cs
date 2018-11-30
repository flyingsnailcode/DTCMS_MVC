using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 自定义参数
    /// </summary>
    public partial class article_attribute : Services<Model.article_attribute>
    {
        private DAL.article_attribute dal = new DAL.article_attribute(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        public List<Model.article_attribute> GetList(int channel_id, int id)
        {
            return dal.GetList(channel_id, id);
        }
        #endregion
    }
}
