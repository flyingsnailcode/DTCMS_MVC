using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;
using DTcms.Cache.Factory;

namespace DTcms.BLL
{
	/// <summary>
	/// 扩展属性表
	/// </summary>
	public partial class article_category : Services<Model.article_category>
    {
        private DAL.article_category dal = new DAL.article_category(siteConfig.sysdatabaseprefix);
        //缓存关键词及名称
        private const int cacheTime = 30;  //分钟
        private const string cacheString = "sys_dtcms_article_category_cache";
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string call_index)
        {
            List<Model.article_category> list = GetCache();
            if (list.Count > 0 && null != list.Find(p => p.call_index == call_index))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            List<Model.article_category> list = GetCache();
            if (list.Count > 0 && null != list.Find(p => p.id == id))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public override bool Update(Model.article_category model)
        {
            if (dal.Update(model))
            {
                SetCache(model);
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
                //删除缓存
                ClearCache(id);
                //返回
                return true;
            }
            return false;
        }

        public DataTable GetList(int parent_id, int channel_id, string strWhere)
        {
            return dal.GetList(parent_id, channel_id, strWhere);
        }

        /// <summary>
        /// 返回类别名称
        /// </summary>
        public string GetTitle(int id)
        {
            Model.article_category model = GetCache().Find(p => p.id == id);
            if (model != null)
            {
                return model.title;
            }
            return string.Empty;
        }

        /// <summary>
        /// 取得该频道下所有类别列表
        /// </summary>
        /// <param name="parent_id">父ID</param>
        /// <param name="channel_name">频道名称</param>
        /// <returns>DataTable</returns>
        public DataTable GetList(int parent_id, string channel_name)
        {
            int channel_id = new BLL.site_channel().GetChannelId(channel_name);
            return dal.GetList(parent_id, channel_id);
        }

        /// <summary>
        /// 取得该频道下所有类别列表
        /// </summary>
        /// <param name="parent_id">父ID</param>
        /// <param name="channel_id">频道ID</param>
        /// <returns>DataTable</returns>
        public DataTable GetList(int parent_id, int channel_id)
        {
            return dal.GetList(parent_id, channel_id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.article_category model, int role_id)
        {
            int id = dal.Add(model, role_id);
            if (id > 0)
            {
                //更新缓存
                model.id = id;
                SetCache(model);
            }
            return id;
        }

        /// <summary>
        /// 根据频道ID、ID得到一个对象实体
        /// </summary>
        /// <param name="channel_id">频道ID</param>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public Model.article_category GetModel(int channel_id, int id)
        {
            return GetCache().Find(p => p.channel_id == channel_id && p.id == id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public override Model.article_category GetModel(int id)
        {
            return GetCache().Find(p => p.id == id);
        }

        /// <summary>
        /// 根据频道ID、调用名得到一个对象实体
        /// </summary>
        /// <param name="channel_id">频道ID</param>
        /// <param name="call_index">调用名称</param>
        /// <returns></returns>
        public Model.article_category GetModel(int channel_id, string call_index)
        {
            return GetCache().Find(p => p.channel_id == channel_id && p.call_index == call_index);
        }

        /// <summary>
        /// 取得该频道指定类别下的列表
        /// </summary>
        /// <param name="parent_id">父ID</param>
        /// <param name="channel_id">频道ID</param>
        /// <returns></returns>
        public DataTable GetChildList(int parent_id, int channel_id)
        {
            return dal.GetChildList(parent_id, channel_id);
        }

        /// <summary>
        /// 取得该频道指定类别下的列表
        /// </summary>
        /// <param name="parent_id"></param>
        /// <param name="channel_name"></param>
        /// <returns></returns>
        public DataTable GetChildList(int parent_id, string channel_name)
        {
            int channel_id = new BLL.site_channel().GetChannelId(channel_name);
            return dal.GetChildList(parent_id, channel_id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.article_category GetModel(string call_index)
        {
            return GetCache().Find(p => p.call_index == call_index);
        }

        /// <summary>
        /// 取得父节点的ID
        /// </summary>
        public int GetParentId(int id)
        {
            Model.article_category model = GetCache().Find(p => p.id == id);
            if (model != null)
            {
                return model.parent_id;
            }
            return 0;
        }

        public int GetCount(string channel_name, int category_id)
        {
            return dal.GetCount(channel_name, category_id);
        }

        /// <summary>
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(int id, string strValue)
        {
            if (dal.UpdateFile("id="+ id, strValue))
            {
                Model.article_category model = dal.Get(id);
                if (model != null)
                {
                    SetCache(model);
                }
                return true;
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
        private void SetCache(Model.article_category model)
        {
            List<Model.article_category> list = GetCache();
            //判断是否存在
            Model.article_category modelt = list.Find(p => p.id == model.id);
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
        private List<Model.article_category> GetCache()
        {
            List<Model.article_category> list = CacheFactory.Cache().GetCache<List<Model.article_category>>(cacheString);
            if (list == null || list.Count == 0)
            {
                list = new List<Model.article_category>();
                //从数据库中读取
                DataTable dt = dal.GetList().Tables[0];
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
        /// <param name="id">缓存名称</param>
        private void ClearCache(int id)
        {
            List<Model.article_category> list = GetCache();
            if (list != null && list.Count > 0)
            {
                Model.article_category model = list.Find(p => p.id == id);
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
        private List<Model.article_category> DataTableToList(DataTable dt)
        {
            List<Model.article_category> modelList = new List<Model.article_category>();
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

