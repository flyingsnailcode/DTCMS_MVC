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
    public class site_channel_thum : Services<Model.site_channel_thum>
    {
        private DAL.site_channel_thum dal = new DAL.site_channel_thum(siteConfig.sysdatabaseprefix);
        private const string cacheString = "sys_dtcms_channel_thum_{0}";
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        #endregion

        #region 扩展
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="channel_id">频道ID</param>
        public void CleanCache(int channel_id)
        {
            CacheFactory.Cache().RemoveCache(string.Format(cacheString, channel_id));
        }

        /// <summary>
        /// 从缓存读取频道生成缩略图列表
        /// </summary>
        /// <param name="channel_id">频道ID</param>
        /// <returns></returns>
        public List<Model.site_channel_thum> GetCacheList(int channel_id)
        {
            List<Model.site_channel_thum> list = CacheFactory.Cache().GetCache<List<Model.site_channel_thum>>(string.Format(cacheString, channel_id));
            if (list == null || list.Count == 0)
            {
                list = new List<Model.site_channel_thum>();
                DataTable dt = dal.GetList(0, "is_lock=0 and channel_id=" + channel_id, "id asc").Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new Model.site_channel_thum()
                    {
                        id = int.Parse(dr["id"].ToString()),
                        channel_id = channel_id,
                        title = dr["width"].ToString(),
                        class_id = Utils.StrToInt(dr["class_id"].ToString(), 0),
                        width = Utils.StrToInt(dr["width"].ToString(), 0),
                        height = Utils.StrToInt(dr["height"].ToString(), 0),
                        typeid = Utils.StrToInt(dr["typeid"].ToString(), 0),
                        is_lock = Utils.StrToInt(dr["is_lock"].ToString(), 0),
                        add_time = Utils.StrToDateTime(dr["add_time"].ToString())
                    });
                }
                CacheFactory.Cache().WriteCache(list, string.Format(cacheString, channel_id), 30);
            }
            return list;
        }
        #endregion
    }
}
