using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// ֧����ʽ
    /// </summary>
    public partial class payment : Services<Model.payment>
    {
        private DAL.payment dal = new DAL.payment(siteConfig.sysdatabaseprefix);
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
        /// <summary>
        /// ��ȡվ��δ�������
        /// </summary>
        public DataSet GetList(int site_id, int payment_id)
        {
            return dal.GetList(site_id, payment_id);
        }
        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }
        #endregion
    }
}