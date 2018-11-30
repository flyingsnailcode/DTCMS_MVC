using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
    /// <summary>
    /// 积分记录日志
    /// </summary>
    public partial class user_point_log : Services<Model.user_point_log>
    {
        private DAL.user_point_log dal = new DAL.user_point_log(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.user_point_log> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 同一天内是否有签到过
        /// </summary>
        public bool ExistsDay(string username, int type)
        {
            return dal.ExistsDay(username, type);
        }

        /// <summary>
        /// 根据删除一条数据
        /// </summary>
        public bool Delete(int id, string user_name)
        {
            return dal.Delete(id, user_name);
        }

        /// <summary>
        /// 增加积分及检查升级
        /// </summary>
        /// <param name="user_id">用户ID</param>
        /// <param name="user_name">用户名</param>
        /// <param name="value">积分值可为正负</param>
        /// <param name="remark">备注</param>
        /// <param name="is_upgrade">是否检查升级</param>
        public int Add(int user_id, string user_name, int value, int type, string remark, bool is_upgrade)
        {
            Model.user_point_log model = new Model.user_point_log();
            model.user_id = user_id;
            model.user_name = user_name;
            model.value = value;
            model.remark = remark;
            model.type = type;
            int result = dal.Add(model, is_upgrade);
            if (is_upgrade && value > 0 && result > 0)
            {
                new BLL.users().Upgrade(user_id);
            }
            return result;
        }
        #endregion
    }
}