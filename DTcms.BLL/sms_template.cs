using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 手机短信模板
    /// </summary>
    public partial class sms_template : Services<Model.sms_template>
    {
        private DAL.sms_template dal = new DAL.sms_template();
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        #endregion
    }
}