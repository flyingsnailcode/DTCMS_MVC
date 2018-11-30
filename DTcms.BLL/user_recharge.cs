using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// 用户充值表
	/// </summary>
	public partial class user_recharge : Services<Model.user_recharge>
    {
        private DAL.user_recharge dal = new DAL.user_recharge(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 根据单号得到一个对象实体
        /// </summary>
        public Model.user_recharge GetModel(string recharge_no)
        {
            return dal.GetModel(recharge_no);
        }

        /// <summary>
        /// 根据充值单号获取支付方式ID
        /// </summary>
        public int GetPaymentId(string recharge_no)
        {
            return dal.GetPaymentId(recharge_no);
        }
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.user_recharge> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 直接充值订单
        /// </summary>
        public bool Recharge(Model.user_recharge model)
        {
            bool result = dal.Recharge(model);
            //冲值，自动升级级别
            if (result)
            {
                new BLL.users().Upgrade(model.user_id, model.amount);
            }
            return result;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(int user_id, string user_name, string recharge_no, int payment_id, decimal amount)
        {
            Model.user_recharge model = new Model.user_recharge();
            model.user_id = user_id;
            model.user_name = user_name;
            model.recharge_no = recharge_no;
            model.payment_id = payment_id;
            model.amount = amount;
            model.status = 0;
            model.add_time = DateTime.Now;
            return dal.Add(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id, string user_name)
        {
            return dal.Delete(id, user_name);
        }

        /// <summary>
        /// 确认充值订单
        /// </summary>
        public bool Confirm(string recharge_no)
        {
            #region 根据冲值金额自动升级
            //获取冲值订单
            Model.user_recharge model = dal.GetModel(recharge_no);
            if (null == model)
            {
                return false;
            }
            new BLL.users().Upgrade(model.user_id, model.amount);
            #endregion

            return dal.Confirm(recharge_no);
        }
        #endregion
    }
}

