using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// �ʼ�ģ��
    /// </summary>
    public partial class mail_template : Services<Model.mail_template>
    {
        private DAL.mail_template dal = new DAL.mail_template();
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        #endregion
    }
}