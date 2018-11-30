using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 用户登录日志
    /// </summary>
    public partial class user_login_log : Services<Model.user_login_log>
    {
        private DAL.user_login_log dal = new DAL.user_login_log(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 同一天内是否有登录过
        /// </summary>
        public bool ExistsDay(string username)
        {
            return dal.ExistsDay(username);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(int user_id, string user_name, string remark)
        {
            Model.user_login_log model = new Model.user_login_log();
            model.user_id = user_id;
            model.user_name = user_name;
            model.remark = remark;
            model.login_ip = DTRequest.GetIP();
            model.login_time = DateTime.Now;
            return dal.Add(model);
        }
        #endregion
    }
}