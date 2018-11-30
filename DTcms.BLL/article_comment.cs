using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// 内容评论
	/// </summary>
	public partial class article_comment : Services<Model.article_comment>
    {
        private DAL.article_comment dal = new DAL.article_comment(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.article_comment> GetList(string channelName, int user_id, bool relation, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            int channelId = new BLL.site_channel().GetChannelId(channelName);//查询频道ID
            return dal.GetList(channelName, channelId, user_id, relation, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }
        
        /// <summary>
		/// 增加一条数据
		/// </summary>
		public override int Add(Model.article_comment model)
        {
            return dal.Add(model);
        }
        #endregion
    }
}

