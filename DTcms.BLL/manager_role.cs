using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;
using DTcms.Cache.Factory;

namespace DTcms.BLL
{
    /// <summary>
    /// 管理角色
    /// </summary>
    public partial class manager_role : Services<Model.manager_role>
    {
        private DAL.manager_role dal = new DAL.manager_role(siteConfig.sysdatabaseprefix);
        //缓存关键词及名称
        private const int cacheTime = 30;  //分钟
        private const string cacheString = "sys_dtcms_manager_role_n{0}_cache";
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public override Model.manager_role GetModel(int id)
        {
            return GetCache(id);
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

                return true;
            }
            return false;
        }

        /// <summary>
        /// 返回角色名称
        /// </summary>
        public string GetTitle(int id)
        {
            return dal.GetTitle(id);
        }

        /// <summary>
        /// 检查是否有权限
        /// </summary>
        public bool Exists(int role_id, string nav_name, string action_type)
        {
            Model.manager_role model;
            return Exists(role_id, nav_name, action_type, out model);
        }
        /// <summary>
        /// 检查是否有权限
        /// </summary>
        public bool Exists(int role_id, string nav_name, string action_type, out Model.manager_role model)
        {
            model = GetCache(role_id);
            if (model != null)
            {
                if (model.role_type == 1)
                {
                    return true;
                }
                Model.manager_role_value modelt = model.manager_role_values.Find(p => p.nav_name == nav_name && p.action_type == action_type);
                if (modelt != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public override int Add(Model.manager_role model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public override bool Update(Model.manager_role model)
        {
            if (dal.Update(model))
            {
                //删除缓存
                ClearCache(model.id);
                return true;
            }
            return false;
        }
        #endregion

        #region 缓存方法===============================
        /// <summary>
        /// 获取字典
        /// </summary>
        /// <returns></returns>
        private Model.manager_role GetCache(int id)
        {
            Model.manager_role model = CacheFactory.Cache().GetCache<Model.manager_role>(string.Format(cacheString, id));
            if (model == null)
            {
                model = dal.Get(id);
                if (model != null)
                {
                    model.manager_role_values = new BLL.manager_role_value().GetModelList(0, "role_id=" + id, "", "");
                    CacheFactory.Cache().WriteCache(model, string.Format(cacheString, id), cacheTime);
                }
            }
            return model;
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">缓存名称</param>
        private void ClearCache(int id)
        {
            CacheFactory.Cache().RemoveCache(string.Format(cacheString, id));
        }
        #endregion
    }
}