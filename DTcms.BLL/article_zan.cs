using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// ��������
	/// </summary>
	public partial class article_zan : Services<Model.article_zan>
    {
        private DAL.article_zan dal = new DAL.article_zan(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
       
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
		public override int Add(Model.article_zan model)
        {
            return dal.Add(model);
        }
        #endregion
    }
}

