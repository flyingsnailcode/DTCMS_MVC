using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
    /// <summary>
    /// OAuth授权用户信息
    /// </summary>
    public partial class user_oauth : Services<Model.user_oauth>
    {
        private DAL.user_oauth dal = new DAL.user_oauth(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 根据开放平台和openid返回一个实体
        /// </summary>
        public Model.user_oauth GetModel(string oauth_name, string oauth_openid)
        {
            return dal.GetModel(oauth_name, oauth_openid);
        }
        #endregion
    }
}