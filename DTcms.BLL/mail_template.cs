using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 邮件模板
    /// </summary>
    public partial class mail_template : Services<Model.mail_template>
    {
        private DAL.mail_template dal = new DAL.mail_template();
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        #endregion
    }
}