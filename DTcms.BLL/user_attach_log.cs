using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
    /// <summary>
    /// 用户下载记录
    /// </summary>
    public partial class user_attach_log : Services<Model.user_attach_log>
    {
        private DAL.user_attach_log dal = new DAL.user_attach_log();
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        #endregion
    }
}