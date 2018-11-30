using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;
using DTcms.Cache.Factory;

namespace DTcms.BLL
{
    /// <summary>
    /// ϵͳƵ����
    /// </summary>
    public partial class site_channel : Services<Model.site_channel>
    {
        private DAL.site_channel dal = new DAL.site_channel(siteConfig.sysdatabaseprefix);
        //����ؼ��ʼ�����
        private const int cacheTime = 30;  //����
        private const string cacheString = "sys_dtcms_channel_cache";
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        /// <summary>
        /// ����վ��ID
        /// </summary>
        public int GetSiteId(int id)
        {
            return dal.GetSiteId(id);
        }

        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public override Model.site_channel GetModel(int id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public Model.site_channel GetModel(string channel_name)
        {
            return dal.GetModel(channel_name);
        }

        /// <summary>
        /// ����Ƶ������
        /// </summary>
        public string GetChannelName(int id)
        {
            return dal.GetChannelName(id);
        }

        /// <summary>
        /// ����Ƶ������
        /// </summary>
        public string GetChannelTitle(int id)
        {
            return dal.GetChannelTitle(id);
        }

        /// <summary>
        /// ����Ƶ��ID
        /// </summary>
        public int GetChannelId(string name)
        {
            return dal.GetChannelId(name);
        }

        /// <summary>
        /// ����һ������
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
        /// ����һ������
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
        /// ɾ��һ������
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
        /// �ӻ�����ȡ������Ƶ���ֵ�
        /// </summary>
        public Dictionary<int, string> GetListAll()
        {
            Dictionary<int, string> dic = CacheHelper.Get<Dictionary<int, string>>(DTKeys.CACHE_SITE_CHANNEL_LIST);//�ӻ���ȡ��
            //��������ѹ���������ݿ�����ȡ��
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
                    CacheHelper.Insert(DTKeys.CACHE_SITE_CHANNEL_LIST, dic, 10);//����д�뻺��
                }
            }
            return dic;
        }

        /// <summary>
        /// ��ѯƵ�������Ƿ����
        /// </summary>
        public bool Exists(string name)
        {
            //��վ��Ŀ¼�µ�һ���ļ����Ƿ�ͬ��
            if (DirPathExists(siteConfig.webpath, name))
            {
                return true;
            }
            //��վ��aspxĿ¼�µ�һ���ļ����Ƿ�ͬ��
            if (DirPathExists(siteConfig.webpath + "/" + DTKeys.DIRECTORY_REWRITE_MVC + "/", name))
            {
                return true;
            }
            //����ڵ�Ƶ�������Ƿ�ͬ��
            List<Model.site_channel> list = GetCache();
            if (list.Count > 0 && null != list.Find(p => p.name == name))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region ˽�з���===============================
        /// <summary>
        /// �������Ŀ¼����ָ��·���µ�һ��Ŀ¼�Ƿ�ͬ��
        /// </summary>
        /// <param name="dirPath">ָ����·��</param>
        /// <param name="build_path">����Ŀ¼��</param>
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

        #region ���淽��===============================
        /// <summary>
        /// ��ȡ��վʵ����
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        private void SetCache(Model.site_channel model)
        {
            List<Model.site_channel> list = GetCache();
            //�ж��Ƿ����
            Model.site_channel modelt = list.Find(p => p.id == model.id);
            if (modelt != null)
            {
                //���Ƴ�
                list.Remove(modelt);
            }
            list.Add(model);
            //����д�뻺��
            CacheFactory.Cache().WriteCache(list, cacheString, cacheTime);
        }

        /// <summary>
        /// ��ȡ�ֵ�
        /// </summary>
        /// <returns></returns>
        private List<Model.site_channel> GetCache()
        {
            List<Model.site_channel> list = CacheFactory.Cache().GetCache<List<Model.site_channel>>(cacheString);
            if (list == null || list.Count == 0)
            {
                list = new List<Model.site_channel>();
                //�����ݿ��ж�ȡ
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
        /// ɾ������
        /// </summary>
        private void ClearAll()
        {
            CacheFactory.Cache().RemoveCache(cacheString);
        }

        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="id">��������</param>
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
            //����д�뻺��
            CacheFactory.Cache().WriteCache(list, cacheString, cacheTime);
        }
        #endregion

        #region ˽�з���===============================
        /// <summary>
        /// ��������б�
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