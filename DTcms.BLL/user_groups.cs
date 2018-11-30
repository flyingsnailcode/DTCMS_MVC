using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
    /// <summary>
    /// �û����
    /// </summary>
    public partial class user_groups : Services<Model.user_groups>
    {
        private DAL.user_groups dal = new DAL.user_groups(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        #endregion

        #region ��չ����===============================
        /// <summary>
        /// ��ȡ��Ա���ۿ�
        /// </summary>
        public int GetDiscount(int id)
        {
            return dal.GetDiscount(id);
        }

        /// <summary>
        /// �����û�������
        /// </summary>
        public string GetTitle(int id)
        {
            return dal.GetTitle(id);
        }

        /// <summary>
        /// ȡ��Ĭ�����ʵ��
        /// </summary>
        public Model.user_groups GetDefault()
        {
            return dal.GetDefault();
        }

        /// <summary>
        /// ���ݾ���ֵ�������������ʵ��
        /// </summary>
        public Model.user_groups GetUpgrade(int group_id, int exp)
        {
            return dal.GetUpgrade(group_id, exp);
        }

        /// <summary>
        /// ���ݳ�ֵ�������������ʵ��
        /// </summary>
        public Model.user_groups GetUpgradePrice(int group_id, decimal price)
        {
            return dal.GetUpgradePrice(group_id, price);
        }
        #endregion
    }
}