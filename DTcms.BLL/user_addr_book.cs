using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// 收货地址簿
	/// </summary>
	public partial class user_addr_book : Services<Model.user_addr_book>
    {
        private DAL.user_addr_book dal = new DAL.user_addr_book(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.user_addr_book> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id, string user_name)
        {
            return dal.Exists(id, user_name);
        }

        /// <summary>
        /// 根据用户名删除一条数据
        /// </summary>
        public bool Delete(int id, string user_name)
        {
            return dal.Delete(id, user_name);
        }
        #endregion

        #region 扩展方法============================
        /// <summary>
        /// 返回默认的地址
        /// </summary>
        public Model.user_addr_book GetDefault(string user_name)
        {
            return dal.GetDefault(user_name);
        }

        /// <summary>
        /// 设置为默认的收货地址
        /// </summary>
        public void SetDefault(int id, string user_name)
        {
            dal.SetDefault(id, user_name);
        }
        #endregion
    }
}

