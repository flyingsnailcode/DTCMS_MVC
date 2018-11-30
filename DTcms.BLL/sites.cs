using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using DTcms.Common;
using DTcms.Cache.Factory;

namespace DTcms.BLL
{
	/// <summary>
	/// 站点管理
	/// </summary>
	public partial class sites : Services<Model.sites>
    {
        private DAL.sites dal = new DAL.sites(siteConfig.sysdatabaseprefix);
        //缓存关键词及名称
        private const int cacheTime = 30;  //分钟
        private const string cacheString = "sys_dtcms_channel_site_cache";
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 返回站点的生成目录名
        /// </summary>
        public string GetBuildPath(int id)
        {
            return dal.GetBuildPath(id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public override int Add(Model.sites model)
        {
            int id = dal.Add(model);
            if (id > 0)
            {
                //删除域名缓存
                CacheHelper.Remove(DTKeys.CACHE_SITE_HTTP_DOMAIN);
                //更新缓存
                model.id = id;
                SetCache(model);
            }
            return id;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public override bool Update(Model.sites model)
        {
            Model.sites modelt = dal.Get(model.id);
            if (modelt == null)
            {
                return false;
            }
            if (dal.Update(model, modelt.build_path))
            {
                if (modelt.build_path.ToLower() != model.build_path.ToLower())
                {
                    //更改频道分类对应的目录名称
                    Utils.MoveDirectory(siteConfig.webpath + DTKeys.DIRECTORY_REWRITE_MVC + "/" + modelt.build_path, siteConfig.webpath + DTKeys.DIRECTORY_REWRITE_MVC + "/" + model.build_path);
                    Utils.MoveDirectory(siteConfig.webpath + DTKeys.DIRECTORY_REWRITE_HTML + "/" + modelt.build_path, siteConfig.webpath + DTKeys.DIRECTORY_REWRITE_HTML + "/" + model.build_path);
                    //更新URL
                    BLL.url_rewrite ubll = new url_rewrite();
                    List<Model.url_rewrite> list = ubll.GetListAll().FindAll(p => p.site == modelt.build_path);
                    if (list != null && list.Count > 0)
                    {
                        foreach (Model.url_rewrite m in list)
                        {
                            var t = m;
                            t.site = model.build_path;
                            ubll.Edit(t);
                        }
                    }
                }
                //删除域名缓存
                CacheHelper.Remove(DTKeys.CACHE_SITE_HTTP_DOMAIN);
                //删除URL缓存
                CacheHelper.Remove(DTKeys.CACHE_SITE_HTTP_MODULE + "_" + modelt.build_path);
                //更新缓存
                if (model.is_default > modelt.is_default)
                {
                    CacheFactory.Cache().RemoveCache(cacheString);
                }
                else
                {
                    SetCache(model);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            if (id == 0)
            {
                return false;
            }
            string old_build_path = dal.GetBuildPath(id);
            if (dal.Delete(id))
            {
                //删除域名缓存
                CacheHelper.Remove(DTKeys.CACHE_SITE_HTTP_DOMAIN);
                //删除URL缓存
                CacheHelper.Remove(DTKeys.CACHE_SITE_HTTP_MODULE + "_" + old_build_path);
                //删除缓存
                ClearCache(id);
                //返回
                return true;
            }
            return false;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.sites GetSiteModel(int channel_id)
        {
            int id = new BLL.site_channel().GetSiteId(channel_id);
            if (id > 0)
            {
                return dal.Get(id);
            }
            return null;
        }
        
        /// <summary>
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(int id, string strValue)
        {
            if (dal.UpdateField(id, strValue))
            {
                Model.sites model = dal.Get(id);
                if (model != null)
                {
                    SetCache(model);
                }
            }
            return false;
        }

        /// <summary>
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(string build_path, string strValue)
        {
            if (dal.UpdateField(build_path, strValue))
            {
                Model.sites model = dal.GetModel("build_path='" + build_path + "'", "", "");
                if (model != null)
                {
                    SetCache(model);
                }
            }
            return false;
        }
        /// <summary>
        /// 查询生成目录名是否存在
        /// </summary>
        public bool Exists(string build_path)
        {
            //与站点目录下的一级文件夹是否重名
            if (DirPathExists(siteConfig.webpath, build_path))
            {
                return true;
            }
            //判断文件目录是否存在，不存在则自动创建
            Utils.DirectoryExists(siteConfig.webpath + "/" + DTKeys.DIRECTORY_REWRITE_MVC + "/");
            //与站点aspx目录下的一级文件夹是否重名
            if (DirPathExists(siteConfig.webpath + "/" + DTKeys.DIRECTORY_REWRITE_MVC + "/", build_path))
            {
                return true;
            }
            //与频道名称是否重名
            if (new DAL.site_channel(siteConfig.sysdatabaseprefix).Exists(build_path))
            {
                return true;
            }
            //与其它站点目录是否重名
            List<Model.sites> list = GetCache();
            if (list.Count > 0 && null != list.Find(p => p.build_path == build_path))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public override Model.sites GetModel(int id)
        {
            return GetCache().Find(p => p.id == id);
        }

        /// <summary>
        /// 返回站点名称
        /// </summary>
        public string GetTitle(int id)
        {
            Model.sites model = GetCache().Find(p => p.id == id);
            if (model != null)
            {
                return model.title;
            }
            return string.Empty;
        }

        /// <summary>
        /// 返回站点名称
        /// </summary>
        public string GetTitle(string build_path)
        {
            Model.sites model = GetCache().Find(p => p.build_path == build_path);
            if (model != null)
            {
                return model.title;
            }
            return string.Empty;
        }
        #endregion

        #region 缓存方法================================
        /// <summary>
        /// 获取网站实体类
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        private void SetCache(Model.sites model)
        {
            List<Model.sites> list = GetCache();
            //判断是否存在
            Model.sites modelt = list.Find(p => p.id == model.id);
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
        private List<Model.sites> GetCache()
        {
            List<Model.sites> list = CacheFactory.Cache().GetCache<List<Model.sites>>(cacheString);
            if (list == null || list.Count == 0)
            {
                list = new List<Model.sites>();
                //从数据库中读取
                DataTable dt = dal.GetList(0, "", "sort_id asc,id desc").Tables[0];
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
            List<Model.sites> list = GetCache();
            if (list != null && list.Count > 0)
            {
                Model.sites model = list.Find(p => p.id == id);
                if (model != null)
                {
                    list.Remove(model);
                }
            }
            //重新写入缓存
            CacheFactory.Cache().WriteCache(list, cacheString, cacheTime);
        }
        #endregion

        #region 私有方法================================
        /// <summary>
        /// 获得数据列表
        /// </summary>
        private List<Model.sites> DataTableToList(DataTable dt)
        {
            List<Model.sites> modelList = new List<Model.sites>();
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

        /// <summary>
        /// 检查生成目录名与指定路径下的一级目录是否同名
        /// </summary>
        /// <param name="dirPath">指定的路径</param>
        /// <param name="build_path">生成目录名</param>
        /// <returns>bool</returns>
        private bool DirPathExists(string dirPath, string build_path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Utils.GetMapPath(dirPath));
            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
                if (build_path.ToLower() == dir.Name.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}

