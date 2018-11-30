using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// ΢�Ź���ƽ̨�˻�
	/// </summary>
	public partial class weixin_account : Services<Model.weixin_account>
    {
        private DAL.weixin_account dal = new DAL.weixin_account(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��������================================
        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }
        #endregion

        #region ��չ����================================
        /// <summary>
        /// �����˻�����
        /// </summary>
        public string GetName(int id)
        {
            return dal.GetName(id);
        }

        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public string GetToken(int id)
        {
            return dal.GetToken(id);
        }

        /// <summary>
        /// �����˻���ԭʼID�Ƿ��Ӧ
        /// </summary>
        public bool ExistsOriginalId(int id, string originalid)
        {
            return dal.ExistsOriginalId(id, originalid);
        }
        #endregion
    }
}

