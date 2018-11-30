using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// �û�����Ϣ
    /// </summary>
    public partial class user_message : Services<Model.user_message>
    {
        private DAL.user_message dal = new DAL.user_message(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        /// <summary>
        /// ����һ������
        /// </summary>
        public int Add(int type, string post_user_name, string accept_user_name, string title, string content)
        {
            Model.user_message model = new Model.user_message();
            model.type = type;
            model.post_user_name = post_user_name;
            model.accept_user_name = accept_user_name;
            model.title = title;
            model.content = content;
            return Add(model);
        }

        /// <summary>
        /// �����û���ɾ��һ������
        /// </summary>
        public bool Delete(int id, string user_name)
        {
            return dal.Delete(id, user_name);
        }

        /// <summary>
        /// ��ò�ѯ��ҳ����
        /// </summary>
        public List<Model.user_message> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        #endregion
    }
}