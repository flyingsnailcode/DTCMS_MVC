using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    ///站点支付方式表
    /// </summary>
    public partial class site_payment : Services<Model.site_payment>
    {
        private DAL.site_payment dal = new DAL.site_payment(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }

        #region 基本方法================================
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }
        
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 返回标题名称
        /// </summary>
        public string GetTitle(int id)
        {
            return dal.GetTitle(id);
        }
        
        /// <summary>
        /// 获取支付平台实体
        /// </summary>
        public Model.payment GetPaymentModel(int id)
        {
            return dal.GetPaymentModel(id);
        }
        #endregion
    }
}