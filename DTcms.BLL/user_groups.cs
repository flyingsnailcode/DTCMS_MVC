using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
    /// <summary>
    /// 用户组别
    /// </summary>
    public partial class user_groups : Services<Model.user_groups>
    {
        private DAL.user_groups dal = new DAL.user_groups(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        #endregion

        #region 扩展方法===============================
        /// <summary>
        /// 获取会员组折扣
        /// </summary>
        public int GetDiscount(int id)
        {
            return dal.GetDiscount(id);
        }

        /// <summary>
        /// 返回用户组名称
        /// </summary>
        public string GetTitle(int id)
        {
            return dal.GetTitle(id);
        }

        /// <summary>
        /// 取得默认组别实体
        /// </summary>
        public Model.user_groups GetDefault()
        {
            return dal.GetDefault();
        }

        /// <summary>
        /// 根据经验值返回升级的组别实体
        /// </summary>
        public Model.user_groups GetUpgrade(int group_id, int exp)
        {
            return dal.GetUpgrade(group_id, exp);
        }

        /// <summary>
        /// 根据充值金额返回升级的组别实体
        /// </summary>
        public Model.user_groups GetUpgradePrice(int group_id, decimal price)
        {
            return dal.GetUpgradePrice(group_id, price);
        }
        #endregion
    }
}