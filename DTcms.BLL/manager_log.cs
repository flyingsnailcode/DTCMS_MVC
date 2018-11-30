using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 管理日志
    /// </summary>
    public partial class manager_log : Services<Model.manager_log>
    {
        private DAL.manager_log dal = new DAL.manager_log(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 删除7天前的日志数据
        /// </summary>
        public int Delete(int dayCount)
        {
            return dal.Delete(dayCount);
        }
        /// <summary>
        /// 根据用户名返回上一次登录记录
        /// </summary>
        public Model.manager_log GetModel(string user_name, int top_num, string action_type)
        {
            return dal.GetModel(user_name, top_num, action_type);
        }
        /// <summary>
        /// 增加管理日志
        /// </summary>
        /// <param name="用户id"></param>
        /// <param name="用户名"></param>
        /// <param name="操作类型"></param>
        /// <param name="备注"></param>
        /// <returns></returns>
        public int Add(int user_id, string user_name, string action_type, string remark)
        {
            Model.manager_log manager_log_model = new Model.manager_log();
            manager_log_model.user_id = user_id;
            manager_log_model.user_name = user_name;
            manager_log_model.action_type = action_type;
            manager_log_model.remark = remark;
            manager_log_model.user_ip = DTRequest.GetIP();
            return dal.Add(manager_log_model);
        }
        #endregion
    }
}
