using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;
using DTcms.Cache.Factory;

namespace DTcms.BLL
{
    /// <summary>
    /// 系统频道表
    /// </summary>
    public partial class site_channel : Services<Model.site_channel>
    {
        private DAL.site_channel dal = new DAL.site_channel(siteConfig.sysdatabaseprefix);
        //缓存关键词及名称
        private const int cacheTime = 30;  //分钟
        private const string cacheString = "sys_dtcms_channel_cache";
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 返回站点ID
        /// </summary>
        public int GetSiteId(int id)
        {
            return dal.GetSiteId(id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public override Model.site_channel GetModel(int id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.site_channel GetModel(string channel_name)
        {
            return dal.GetModel(channel_name);
        }

        /// <summary>
        /// 返回频道名称
        /// </summary>
        public string GetChannelName(int id)
        {
            return dal.GetChannelName(id);
        }

        /// <summary>
        /// 返回频道标题
        /// </summary>
        public string GetChannelTitle(int id)
        {
            return dal.GetChannelTitle(id);
        }

        /// <summary>
        /// 返回频道ID
        /// </summary>
        public int GetChannelId(string name)
        {
            return dal.GetChannelId(name);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public override int Add(Model.site_channel model)
        {
            int id = dal.Add(model);
            if (id > 0)
            {
                CacheHelper.Remove(DTKeys.CACHE_SITE_CHANNEL_LIST);
            }
            return id;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public override bool Update(Model.site_channel model)
        {
            if (dal.Update(model))
            {
                CacheHelper.Remove(DTKeys.CACHE_SITE_CHANNEL_LIST);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            if (dal.Delete(id))
            {
                CacheHelper.Remove(DTKeys.CACHE_SITE_CHANNEL_LIST);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 从缓存中取出所有频道字典
        /// </summary>
        public Dictionary<int, string> GetListAll()
        {
            Dictionary<int, string> dic = CacheHelper.Get<Dictionary<int, string>>(DTKeys.CACHE_SITE_CHANNEL_LIST);//从缓存取出
            //如果缓存已过期则从数据库里面取出
            if (dic == null)
            {
                dic = new Dictionary<int, string>();
                DataTable dt = dal.GetList(string.Empty).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dic.Add(int.Parse(dr["id"].ToString()), dr["name"].ToString());
                    }
                    CacheHelper.Insert(DTKeys.CACHE_SITE_CHANNEL_LIST, dic, 10);//重新写入缓存
                }
            }
            return dic;
        }

        /// <summary>
        /// 查询频道名称是否存在
        /// </summary>
        public bool Exists(string name)
        {
            //与站点目录下的一级文件夹是否同名
            if (DirPathExists(siteConfig.webpath, name))
            {
                return true;
            }
            //与站点aspx目录下的一级文件夹是否同名
            if (DirPathExists(siteConfig.webpath + "/" + DTKeys.DIRECTORY_REWRITE_MVC + "/", name))
            {
                return true;
            }
            //与存在的频道名称是否同名
            List<Model.site_channel> list = GetCache();
            if (list.Count > 0 && null != list.Find(p => p.name == name))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 私有方法===============================
        /// <summary>
        /// 检查生成目录名与指定路径下的一级目录是否同名
        /// </summary>
        /// <param name="dirPath">指定的路径</param>
        /// <param name="build_path">生成目录名</param>
        /// <returns>bool</returns>
        private bool DirPathExists(string dirPath, string build_path)
        {
            if (Directory.Exists(Utils.GetMapPath(dirPath)))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Utils.GetMapPath(dirPath));
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    if (build_path.ToLower() == dir.Name.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region 缓存方法===============================
        /// <summary>
        /// 获取网站实体类
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        private void SetCache(Model.site_channel model)
        {
            List<Model.site_channel> list = GetCache();
            //判断是否存在
            Model.site_channel modelt = list.Find(p => p.id == model.id);
            if (modelt != null)
            {
                //先移除
                list.Remove(modelt);
            }
            list.Add(model);
            //重新写入缓存
            CacheFactory.Cache().WriteCache(list, cacheString, cacheTime);
        }

        /// <summary>
        /// 获取字典
        /// </summary>
        /// <returns></returns>
        private List<Model.site_channel> GetCache()
        {
            List<Model.site_channel> list = CacheFactory.Cache().GetCache<List<Model.site_channel>>(cacheString);
            if (list == null || list.Count == 0)
            {
                list = new List<Model.site_channel>();
                //从数据库中读取
                DataTable dt = dal.GetList(0, "", "id asc").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    list = DataTableToList(dt);
                    CacheFactory.Cache().WriteCache(list, cacheString, cacheTime);
                }
            }
            return list;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        private void ClearAll()
        {
            CacheFactory.Cache().RemoveCache(cacheString);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">缓存名称</param>
        private void ClearCache(int id)
        {
            List<Model.site_channel> list = GetCache();
            if (list != null && list.Count > 0)
            {
                Model.site_channel model = list.Find(p => p.id == id);
                if (model != null)
                {
                    list.Remove(model);
                }
            }
            //重新写入缓存
            CacheFactory.Cache().WriteCache(list, cacheString, cacheTime);
        }
        #endregion

        #region 私有方法===============================
        /// <summary>
        /// 获得数据列表
        /// </summary>
        private List<Model.site_channel> DataTableToList(DataTable dt)
        {
            List<Model.site_channel> modelList = new List<Model.site_channel>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                for (int n = 0; n < rowsCount; n++)
                {
                    modelList.Add(dal.DataRowToModel(dt.Rows[n]));
                }
            }
            return modelList;
        }
        #endregion
    }
}