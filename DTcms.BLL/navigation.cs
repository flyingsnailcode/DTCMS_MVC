using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// ֧����ʽ
    /// </summary>
    public partial class navigation : Services<Model.navigation>
    {
        private DAL.navigation dal = new DAL.navigation(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        /// <summary>
        /// ��ѯ�����Ƿ����
        /// </summary>
        public bool Exists(string name)
        {
            return dal.Exists(name);
        }
        /// <summary>
        /// ȡ����������б�
        /// </summary>
        /// <param name="parent_id">��ID</param>
        /// <param name="nav_type">�������</param>
        /// <returns>DataTable</returns>
        public DataTable GetList(int parent_id, string nav_type)
        {
            return dal.GetList(parent_id, nav_type);
        }

        /// <summary>
        /// ����ͼ��Ŀ¼
        /// </summary>
        /// <param name="oldName">ԭĿ¼</param>
        /// <param name="newName">��Ŀ¼</param>
        public void updateicon(string oldName, string newName)
        {
            dal.updateicon(oldName, newName);
        }

        /// <summary>
		/// ɾ��һ������
		/// </summary>
		public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// ������ϵͳĬ�ϵ���
        /// </summary>
        /// <param name="parent_name">����������</param>
        /// <param name="nav_name">��������</param>
        /// <param name="title">��������</param>
        /// <param name="link_url">���ӵ�ַ</param>
        /// <param name="sort_id">��������</param>
        /// <param name="channel_id">����Ƶ��ID</param>
        /// <param name="action_type">����Ȩ����Ӣ�Ķ��ŷָ���</param>
        /// <returns>int</returns>
        public int Add(string parent_name, string nav_name, string title, string link_url, int sort_id, int channel_id, string action_type)
        {
            return dal.Add(parent_name, nav_name, title, link_url, sort_id, channel_id, action_type);
        }
        #endregion
    }
}