using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// ���ظ���
    /// </summary>
    public partial class article_attach : Services<Model.article_attach>
    {
        private DAL.article_attach dal = new DAL.article_attach(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        public List<Model.article_attach> GetList(int channel_id, int id)
        {
            return dal.GetList(channel_id, id);
        }
        #endregion

        #region ��չ����================================
        /// <summary>
        /// ����û��Ƿ����ع��ø���
        /// </summary>
        public bool ExistsLog(int attach_id, int user_id)
        {
            return dal.ExistsLog(attach_id, user_id);
        }

        /// <summary>
        /// ��ȡ���ش���
        /// </summary>
        public int GetDownNum(int id)
        {
            return dal.GetDownNum(id);
        }

        /// <summary>
        /// ��ȡ�����ش���
        /// </summary>
        public int GetCountNum(int channel_id, int article_id)
        {
            return dal.GetCountNum(channel_id, article_id);
        }

        /// <summary>
        /// ����һ�����ظ�����¼
        /// </summary>
        public int AddLog(int user_id, string user_name, int attach_id, string file_name)
        {
            Model.user_attach_log model = new Model.user_attach_log();
            model.user_id = user_id;
            model.user_name = user_name;
            model.attach_id = attach_id;
            model.file_name = file_name;
            model.add_time = DateTime.Now;
            return dal.AddLog(model);
        }
        
        //ɾ�����µľ��ļ�
        public void DeleteFile(int id, string filePath)
        {
            Model.article_attach model = Get(id);
            if (model != null && model.file_path != filePath)
            {
                FileHelper.DeleteFile(model.file_path);
            }
        }
        #endregion
    }
}