using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// 会员组价格
	/// </summary>
	public partial class user_group_price : Services<Model.user_group_price>
    {
        private DAL.user_group_price dal = new DAL.user_group_price(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
		/// 得到一个对象实体
		/// </summary>
        public Model.user_group_price GetModel(int goods_id, int group_id)
        {
            return dal.GetModel(goods_id, group_id);
        }
        #endregion
    }
}

