using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// 微信公众平台账户
	/// </summary>
	public partial class weixin_account : Services<Model.weixin_account>
    {
        private DAL.weixin_account dal = new DAL.weixin_account(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 基本方法================================
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 返回账户名称
        /// </summary>
        public string GetName(int id)
        {
            return dal.GetName(id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public string GetToken(int id)
        {
            return dal.GetToken(id);
        }

        /// <summary>
        /// 公众账户和原始ID是否对应
        /// </summary>
        public bool ExistsOriginalId(int id, string originalid)
        {
            return dal.ExistsOriginalId(id, originalid);
        }
        #endregion
    }
}

