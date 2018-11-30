using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
    /// ����ظ����
	/// </summary>
	public partial class weixin_request_rule : Services<Model.weixin_request_rule>
    {
        private DAL.weixin_request_rule dal = new DAL.weixin_request_rule(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��������================================
        /// <summary>
        /// ����һ������
        /// </summary>
        public override int Add(Model.weixin_request_rule model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public override bool Update(Model.weixin_request_rule model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public override Model.weixin_request_rule GetModel(int id)
        {
            return dal.GetModel(id);
        }
        
        #endregion

        #region ��չ����================================
        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public Model.weixin_request_rule GetModel(int account_id, int request_type)
        {
            return dal.GetModel(account_id, request_type);
        }
        #endregion

        #region ΢��ͨѶ����===============================
        /// <summary>
        /// �õ�����ID�Իظ�����
        /// </summary>
        public int GetRuleIdAndResponseType(int account_id, string strWhere, out int response_type)
        {
            return dal.GetRuleIdAndResponseType(account_id, strWhere, out response_type);
        }

        /// <summary>
        /// �õ��ؽ��ֲ�ѯ�Ĺ���ID���ظ�����
        /// </summary>
        public int GetKeywordsRuleId(int account_id, string keywords, out int response_type)
        {
            return dal.GetKeywordsRuleId(account_id, keywords, out response_type);
        }
        #endregion
    }
}

