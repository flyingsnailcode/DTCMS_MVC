using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// �������
    /// </summary>
    public partial class express : Services<Model.express>
    {
        private DAL.express dal = new DAL.express(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        /// <summary>
        /// ���ر�������
        /// </summary>
        public string GetTitle(int id)
        {
            return dal.GetTitle(id);
        }
        #endregion
    }
}