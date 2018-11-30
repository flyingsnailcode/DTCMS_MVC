using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// 内容评论
	/// </summary>
	public partial class article_zan : Services<Model.article_zan>
    {
        private DAL.article_zan dal = new DAL.article_zan(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
       
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
		public override int Add(Model.article_zan model)
        {
            return dal.Add(model);
        }
        #endregion
    }
}

