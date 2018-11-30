using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// ������
    /// </summary>
    public partial class orders : Services<Model.orders>
    {
        private DAL.orders dal = new DAL.orders(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        /// <summary>
        /// ���ݶ����Ż�ȡ֧����ʽID
        /// </summary>
        public int GetPaymentId(string order_no)
        {
            return dal.GetPaymentId(order_no);
        }

        /// <summary>
        /// ��ò�ѯ��ҳ����
        /// </summary>
        public List<Model.orders> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public override Model.orders GetModel(int id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// ���ݶ����ŷ���һ��ʵ��
        /// </summary>
        public Model.orders GetModel(string order_no)
        {
            return dal.GetModel(order_no);
        }

        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// �����û�ID�͹�����ƷID�ж��û��Ƿ��Ѿ��������Ʒ ���ö����Ѿ���ɣ�
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="article_id"></param>
        /// <returns></returns>
        public bool Exists(int user_id, int article_id)
        {
            return dal.Exists(user_id, article_id);
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public override int Add(Model.orders model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public override bool Update(Model.orders model)
        {
            //���㶩���ܽ��:��Ʒ�ܽ��+���ͷ���+֧��������
            model.order_amount = model.real_amount + model.express_fee + model.payment_fee + model.invoice_taxes;
            return dal.Update(model);
        }

        /// <summary>
        /// �޸�һ������
        /// </summary>
        public bool UpdateField(string order_no, string strValue)
        {
            return dal.UpdateField(order_no, strValue);
        }
        #endregion
    }
}