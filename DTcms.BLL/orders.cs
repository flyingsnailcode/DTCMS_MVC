using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 订单表
    /// </summary>
    public partial class orders : Services<Model.orders>
    {
        private DAL.orders dal = new DAL.orders(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 根据订单号获取支付方式ID
        /// </summary>
        public int GetPaymentId(string order_no)
        {
            return dal.GetPaymentId(order_no);
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.orders> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public override Model.orders GetModel(int id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// 根据订单号返回一个实体
        /// </summary>
        public Model.orders GetModel(string order_no)
        {
            return dal.GetModel(order_no);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// 根据用户ID和购买商品ID判断用户是否已经购买该商品 （该订单已经完成）
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="article_id"></param>
        /// <returns></returns>
        public bool Exists(int user_id, int article_id)
        {
            return dal.Exists(user_id, article_id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public override int Add(Model.orders model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public override bool Update(Model.orders model)
        {
            //计算订单总金额:商品总金额+配送费用+支付手续费
            model.order_amount = model.real_amount + model.express_fee + model.payment_fee + model.invoice_taxes;
            return dal.Update(model);
        }

        /// <summary>
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(string order_no, string strValue)
        {
            return dal.UpdateField(order_no, strValue);
        }
        #endregion
    }
}