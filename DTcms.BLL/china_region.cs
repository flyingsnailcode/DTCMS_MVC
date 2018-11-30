using System;
using System.Collections.Generic;
using System.Data;
using DTcms.Common;
using DTcms.Cache.Factory;

namespace DTcms.BLL
{
    /// <summary>
    /// 业务逻辑层
    /// </summary>
    public class china_region : Services<Model.china_region>
    {
        private DAL.china_region dal = new DAL.china_region(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 按ID号查询标题
        /// </summary>
        /// <param name="id">ID号</param>
        /// <returns>标题</returns>
        public string GetTitle(int id)
        {
            return dal.GetTitle(id);
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        /// <param name="top"></param>
        /// <param name="strWhere">条件</param>
        /// <param name="filedOrder">排序</param>
        /// <returns>DataTable</returns>
        public DataSet GetList(int top, string strWhere, string filedOrder)
        {
            return dal.GetList(top, strWhere, filedOrder);
        }

        #endregion

        #region 前端调用
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="parent_id">父节点</param>
        private void ClearCache(int parent_id)
        {
            CacheFactory.Cache().RemoveCache(string.Format("sys_dtcms_china_region_n{0}_cache", parent_id));
        }
        public Dictionary<int, string> GetDict(int parent_id)
        {
            string cacheString = string.Format("sys_dtcms_china_region_n{0}_cache", parent_id);
            //从缓存读取
            Dictionary<int, string> dic = CacheFactory.Cache().GetCache<Dictionary<int, string>>(cacheString);
            if (dic == null)
            {
                dic = new Dictionary<int, string>();
                DataTable dt = dal.GetList(0, "is_lock=0 and parent_id=" + parent_id, "sort_id asc,id asc").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    int id = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        id = Utils.StrToInt(dr["id"].ToString(), 0);
                        if (id > 0)
                        {
                            dic.Add(id, dr["title"].ToString());
                        }
                    }
                    CacheFactory.Cache().WriteCache(dic, cacheString, 30);
                }
            }
            return dic;
        }
        #endregion
    }
}
