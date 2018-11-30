using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;
using DTcms.Cache.Factory;

namespace DTcms.BLL
{
    /// <summary>
    /// �����ɫ
    /// </summary>
    public partial class manager_role : Services<Model.manager_role>
    {
        private DAL.manager_role dal = new DAL.manager_role(siteConfig.sysdatabaseprefix);
        //����ؼ��ʼ�����
        private const int cacheTime = 30;  //����
        private const string cacheString = "sys_dtcms_manager_role_n{0}_cache";
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public override Model.manager_role GetModel(int id)
        {
            return GetCache(id);
        }

        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(int id)
        {
            if (dal.Delete(id))
            {
                //ɾ������
                ClearCache(id);

                return true;
            }
            return false;
        }

        /// <summary>
        /// ���ؽ�ɫ����
        /// </summary>
        public string GetTitle(int id)
        {
            return dal.GetTitle(id);
        }

        /// <summary>
        /// ����Ƿ���Ȩ��
        /// </summary>
        public bool Exists(int role_id, string nav_name, string action_type)
        {
            Model.manager_role model;
            return Exists(role_id, nav_name, action_type, out model);
        }
        /// <summary>
        /// ����Ƿ���Ȩ��
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
        /// ����һ������
        /// </summary>
        public override int Add(Model.manager_role model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public override bool Update(Model.manager_role model)
        {
            if (dal.Update(model))
            {
                //ɾ������
                ClearCache(model.id);
                return true;
            }
            return false;
        }
        #endregion

        #region ���淽��===============================
        /// <summary>
        /// ��ȡ�ֵ�
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
        /// ɾ������
        /// </summary>
        /// <param name="id">��������</param>
        private void ClearCache(int id)
        {
            CacheFactory.Cache().RemoveCache(string.Format(cacheString, id));
        }
        #endregion
    }
}