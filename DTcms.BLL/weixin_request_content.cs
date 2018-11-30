using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// ΢������ظ�������
	/// </summary>
	public partial class weixin_request_content : Services<Model.weixin_request_content>
    {
        private DAL.weixin_request_content dal = new DAL.weixin_request_content(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��������================================
        #endregion

        #region ��չ����================================
        /// <summary>
        /// ���ع���ĵ�һ������
        /// </summary>
        public string GetTitle(int rule_id)
        {
            return dal.GetTitle(rule_id);
        }

        /// <summary>
        /// ���ع���ĵ�һ������
        /// </summary>
        public string GetContent(int rule_id)
        {
            return dal.GetContent(rule_id);
        }

        /// <summary>
        /// ���ع����������������
        /// </summary>
        public int GetCount(int rule_id)
        {
            return dal.GetCount(rule_id);
        }
        #endregion
    }
}

