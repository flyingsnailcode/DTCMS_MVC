using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 网站授权
    /// </summary>
    public partial class sn_myaspx : Services<Model.sn_myaspx>
    {
        private DAL.sn_myaspx dal = new DAL.sn_myaspx(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        
        #endregion
    }
}