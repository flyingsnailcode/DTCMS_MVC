using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 下载附件
    /// </summary>
    public partial class article_attach : Services<Model.article_attach>
    {
        private DAL.article_attach dal = new DAL.article_attach(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        public List<Model.article_attach> GetList(int channel_id, int id)
        {
            return dal.GetList(channel_id, id);
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 检查用户是否下载过该附件
        /// </summary>
        public bool ExistsLog(int attach_id, int user_id)
        {
            return dal.ExistsLog(attach_id, user_id);
        }

        /// <summary>
        /// 获取下载次数
        /// </summary>
        public int GetDownNum(int id)
        {
            return dal.GetDownNum(id);
        }

        /// <summary>
        /// 获取总下载次数
        /// </summary>
        public int GetCountNum(int channel_id, int article_id)
        {
            return dal.GetCountNum(channel_id, article_id);
        }

        /// <summary>
        /// 插入一条下载附件记录
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
        
        //删除更新的旧文件
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