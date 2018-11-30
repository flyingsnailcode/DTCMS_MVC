using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// ����ƽ̨AccessTokenֵ
	/// </summary>
	public partial class weixin_access_token : Services<Model.weixin_access_token>
    {
        private DAL.weixin_access_token dal = new DAL.weixin_access_token(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }

        #region ��������================================
        #endregion

        #region ��չ����================================
        /// <summary>
        /// ��������һ������
        /// </summary>
        public int Add(int account_id, string access_token)
        {
            Model.weixin_access_token model = new Model.weixin_access_token();
            model.account_id = account_id;
            model.access_token = access_token;
            model.count = 1;
            model.expires_in = 1200; //1200��
            model.add_time = DateTime.Now;
            return dal.Add(model);
        }

        /// <summary>
        /// �Ƿ���ڸù����˻���¼
        /// </summary>
        public bool ExistsAccount(int account_id)
        {
            return dal.ExistsAccount(account_id);
        }

        /// <summary>
        /// ��ȡ�ù����˻���AccessTokenʵ��
        /// </summary>
        public Model.weixin_access_token GetAccountModel(int account_id)
        {
            return dal.GetAccountModel(account_id);
        }
        #endregion
    }
}

