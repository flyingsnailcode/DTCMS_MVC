using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
    /// <summary>
    /// 用户生成码
    /// </summary>
    public partial class user_code : Services<Model.user_code>
    {
        private DAL.user_code dal = new DAL.user_code(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string type, string user_name)
        {
            return dal.Exists(type, user_name);
        }

        /// <summary>
        /// 返回默认的地址
        /// </summary>
        public Model.user_code GetDefault(string user_name, string code_type, string datepart)
        {
            return dal.GetModel(user_name, code_type, datepart);
        }

        /// <summary>
        /// 根据生成码得到一个对象实体
        /// </summary>
        public Model.user_code GetModel(string str_code)
        {
            return dal.GetModel(str_code);
        }
        #endregion
    }
}