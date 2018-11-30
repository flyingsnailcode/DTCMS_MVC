using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
    /// <summary>
    /// OAuth��Ȩ�û���Ϣ
    /// </summary>
    public partial class user_oauth : Services<Model.user_oauth>
    {
        private DAL.user_oauth dal = new DAL.user_oauth(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        /// <summary>
        /// ���ݿ���ƽ̨��openid����һ��ʵ��
        /// </summary>
        public Model.user_oauth GetModel(string oauth_name, string oauth_openid)
        {
            return dal.GetModel(oauth_name, oauth_openid);
        }
        #endregion
    }
}