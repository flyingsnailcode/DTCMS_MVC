using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// 会员金额日志表
	/// </summary>
	public partial class user_amount_log : Services<Model.user_amount_log>
    {
        private DAL.user_amount_log dal = new DAL.user_amount_log(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.user_amount_log> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(int user_id, string user_name, decimal value, string remark)
        {
            Model.user_amount_log model = new Model.user_amount_log();
            model.user_id = user_id;
            model.user_name = user_name;
            model.value = value;
            model.remark = remark;
            return dal.Add(model);
        }

        /// <summary>
		/// 增加一条数据
		/// </summary>
		public override int Add(Model.user_amount_log model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public override bool Update(Model.user_amount_log model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 根据用户名删除一条数据
        /// </summary>
        public bool Delete(int id, string user_name)
        {
            return dal.Delete(id, user_name);
        }
        #endregion
    }
}

