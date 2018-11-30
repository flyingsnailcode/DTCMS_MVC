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
    ///OAuth应用
    /// </summary>
    public partial class oauth_app : Services<Model.oauth_app>
    {
        private DAL.oauth_app dal = new DAL.oauth_app(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }

        #region 基本方法================================

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.oauth_app GetModel(string api_path)
        {
            return dal.GetModel(api_path);
        }

        /// <summary>
        /// 获取站点未添加数据
        /// </summary>
        public DataSet GetList(int site_id, int oauth_id)
        {
            return dal.GetList(site_id, oauth_id);
        }
        #endregion
    }
}