using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 支付方式
    /// </summary>
    public partial class payment : Services<Model.payment>
    {
        private DAL.payment dal = new DAL.payment(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 返回标题名称
        /// </summary>
        public string GetTitle(int id)
        {
            return dal.GetTitle(id);
        }
        /// <summary>
        /// 获取站点未添加数据
        /// </summary>
        public DataSet GetList(int site_id, int payment_id)
        {
            return dal.GetList(site_id, payment_id);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }
        #endregion
    }
}