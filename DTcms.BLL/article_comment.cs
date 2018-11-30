using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// ��������
	/// </summary>
	public partial class article_comment : Services<Model.article_comment>
    {
        private DAL.article_comment dal = new DAL.article_comment(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        /// <summary>
        /// ��ò�ѯ��ҳ����
        /// </summary>
        public List<Model.article_comment> GetList(string channelName, int user_id, bool relation, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            int channelId = new BLL.site_channel().GetChannelId(channelName);//��ѯƵ��ID
            return dal.GetList(channelName, channelId, user_id, relation, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// ��ò�ѯ��ҳ����
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }
        
        /// <summary>
		/// ����һ������
		/// </summary>
		public override int Add(Model.article_comment model)
        {
            return dal.Add(model);
        }
        #endregion
    }
}

